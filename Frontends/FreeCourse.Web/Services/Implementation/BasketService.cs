using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Basket;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Implementation
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscountService _discountService;

        public BasketService(HttpClient httpClient, IDiscountService discountService)
        {
            _httpClient = httpClient;
            _discountService = discountService;
        }


        /// <summary>
        /// This Method add Items to the basket
        /// </summary>
        /// <param name="basketItemViewModel"></param>
        /// <returns></returns>
        public async Task AddBasketItem(BasketItemViewModel basketItemViewModel)
        {
            var basket = await Get();

            if (basket != null)
            {
                if (!basket.BasketItems.Any(x => x.CourseId == basketItemViewModel.CourseId))
                {
                    basket.BasketItems.Add(basketItemViewModel);
                }
            }
            else
            {
                basket = new BasketViewModel();

                basket.BasketItems.Add(basketItemViewModel);
            }

            await SaveOrUpdate(basket);
        }

        public async Task<bool> ApplyDiscount(string discountCode)
        {
            await CancelApplyDiscount();
            var basket = await Get();
            if (basket == null)
                return false;

            var hasDiscount = await _discountService.GetDiscount(discountCode);
            if (hasDiscount == null)
                return false;

            basket.ApplyDiscount(hasDiscount.Code, hasDiscount.Rate);
            await SaveOrUpdate(basket);
            return true;

        }

        public async Task<bool> CancelApplyDiscount()
        {
            var basket = await Get();
            if (basket == null || basket.DiscountCode == null)
                return false;

            basket.CancelDiscount();
            await SaveOrUpdate(basket);
            return true;
        }


        /// <summary>
        /// This Method delete the basket
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Delete()
        {
            var result = await _httpClient.DeleteAsync("baskets");
            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// This Method get the basket
        /// </summary>
        /// <returns></returns>
        public async Task<BasketViewModel> Get()
        {
            var response = await _httpClient.GetAsync("baskets");
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }
            var basketViewMode = await response.Content.ReadFromJsonAsync<Response<BasketViewModel>>();
            return basketViewMode.Data;
        }


        /// <summary>
        /// Remove Item from Basket
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveBasketItem(string courseId)
        {
            var basket = await Get();
            if (basket == null)
                return false;

            var deleteBasketItem = basket.BasketItems.FirstOrDefault(x => x.CourseId == courseId);
            if (deleteBasketItem == null)
                return false;

            var deleteResult = basket.BasketItems.Remove(deleteBasketItem);
            if (!deleteResult)
                return false;

            if (!basket.BasketItems.Any())
                basket.DiscountCode = null;

            return await SaveOrUpdate(basket);

        }

        /// <summary>
        /// This Method Save or Update course to Basket
        /// </summary>
        /// <param name="basketViewModel"></param>
        /// <returns></returns>
        public async Task<bool> SaveOrUpdate(BasketViewModel basketViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync<BasketViewModel>("baskets", basketViewModel);
            return response.IsSuccessStatusCode;
        }
    }
}

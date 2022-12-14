using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Basket
{
    public class BasketItemViewModel
    {
        public int Quantity { get; set; } = 1;
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountAppliedToPrice;

        public decimal GetCurrentPrice
        {
            get => DiscountAppliedToPrice != null ? DiscountAppliedToPrice.Value : Price;
        }
        public void AppliedDiscount(decimal discountPrice)
        {
            DiscountAppliedToPrice = discountPrice;
        }
    }
}

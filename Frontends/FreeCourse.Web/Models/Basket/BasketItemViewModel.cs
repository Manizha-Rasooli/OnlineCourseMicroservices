using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Basket
{
    public class BasketItemViewModel
    {
        public int Quantity { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountAppliedToPrice { get; set; }

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

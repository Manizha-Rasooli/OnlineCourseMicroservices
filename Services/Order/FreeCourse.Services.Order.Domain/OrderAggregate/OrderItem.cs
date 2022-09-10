using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    /// <summary>
    /// Returns OrderItems
    /// </summary>
    public class OrderItem:Entity
    {
        #region Properties
        public string ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string PictureUrl { get; private set; }
        public Decimal Price { get; private set; }
        #endregion

        #region Constructor Method

        public OrderItem(string productId, string productName, string pictureUrl, decimal price)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
            Price = price;
        }
        #endregion


        /// <summary>
        /// This Method sert or Update OrderItems values
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="pictureUrl"></param>
        /// <param name="price"></param>
        public void UpdateOrderItem(string productName,string pictureUrl, decimal price)
        {
            ProductName = productName;
            PictureUrl = pictureUrl;
            Price = price;
        }
    }
}

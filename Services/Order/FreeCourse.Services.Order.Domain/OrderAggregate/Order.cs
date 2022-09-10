using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    public class Order:Entity, IAggregateRoot
    {
        #region Properties
        public DateTime CreateDateTime { get;private set; }
        public Address Address { get; private set; }
        public string BuyerId  { get; private set; }
        #endregion

        private readonly List<OrderItem> _orderItems; // We can only set Or Fill orderItems from this field
        /// <summary>
        /// Only Read Order Items 
        /// </summary>
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        #region Order Constructor Method
        public Order(string buyerId, Address address)
        {
            _orderItems = new List<OrderItem>();
            CreateDateTime = DateTime.Now;
            BuyerId = buyerId;
            Address = address;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This Method add OrderItems
        /// </summary>
        /// <param name="prodcutId"></param>
        /// <param name="productName"></param>
        /// <param name="price"></param>
        /// <param name="pictureUrl"></param>
        public void AddOrderItem(string prodcutId, string productName, decimal price, string pictureUrl)
        {
            var existProduct = _orderItems.Any(x => x.ProductId == prodcutId);
            if(!existProduct)
            {
                var newOrderItem = new OrderItem(prodcutId, productName, pictureUrl, price);
                _orderItems.Add(newOrderItem);
            }
        }

        /// <summary>
        /// Returns OrderItems Total Price
        /// </summary>
        public decimal GetTotalPrice => _orderItems.Sum(x=>x.Price);
        #endregion
    }
}

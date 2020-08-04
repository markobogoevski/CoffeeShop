using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models.Order
{
    public class OrderItemModel
    {
        [Key]
        public Guid OrderItemId { get; set; }

        public virtual CoffeeModel Coffee { get; set; }

        public virtual OrderModel Order { get; set; }

        public int Quantity { get; set; }
    }
}
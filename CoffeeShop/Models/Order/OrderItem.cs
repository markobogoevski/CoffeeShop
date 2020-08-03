using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models.Order
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; }

        public virtual CoffeeComponentModel Coffee { get; set; }

        public int Quantity { get; set; }
    }
}
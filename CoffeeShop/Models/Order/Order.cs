using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models.Order
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        [Required]
        public DateTime OrderTime { get; set; }

        public int OrderRating { get; set; }

        public virtual ApplicationUser user { get; set; }

        public Order()
        {
            OrderStatus = Enumerations.OrderStatus.PENDING;
            OrderItems = new List<OrderItem>();
        }
    }
}
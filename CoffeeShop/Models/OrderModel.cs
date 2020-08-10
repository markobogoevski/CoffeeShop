namespace CoffeeShop.Models.Order
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OrderModel
    {
        [Key]
        public Guid OrderId { get; set; }

        public virtual List<OrderItemModel> OrderItems { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        [Required]
        public DateTime OrderTime { get; set; }

        public DateTime OrderFinishTime { get; set; }

        public int OrderRating { get; set; }

        public virtual ApplicationUser User { get; set; }

        public OrderModel()
        {
            OrderStatus = Enumerations.OrderStatus.INACTIVE;
            OrderItems = new List<OrderItemModel>();
        }
    }
}
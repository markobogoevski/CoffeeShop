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
        [Display(Name = "Order address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Status of order")]
        public string OrderStatus { get; set; }

        [Required]
        [Display(Name = "Time of order activation")]
        public DateTime OrderTime { get; set; }

        [Display(Name = "Time of order finish")]
        public DateTime OrderFinishTime { get; set; }

        [Display(Name = "Order rating")]
        public int OrderRating { get; set; }

        public virtual ApplicationUser User { get; set; }

        public OrderModel()
        {
            OrderStatus = Enumerations.OrderStatus.INACTIVE;
            OrderItems = new List<OrderItemModel>();
        }
    }
}
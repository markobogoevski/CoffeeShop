namespace CoffeeShop.Models.Order
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OrderItemModel
    {
        [Key]
        public Guid OrderItemId { get; set; }

        public virtual CoffeeModel Coffee { get; set; }

        public virtual OrderModel Order { get; set; }

        [Required]
        [Display(Name = "Quantity of coffee")]
        public int Quantity { get; set; }

        [Display(Name="Coffee size")]
        public string CoffeeSize { get; set; }
    }
}
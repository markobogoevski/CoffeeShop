namespace CoffeeShop.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class IngredientInCoffeeModel
    {
        [Key]
        public Guid IngredientInCoffeeId { get; set; }

        public Guid CoffeeId { get; set; }

        public Guid IngredientId { get; set; }

        public virtual CoffeeModel Coffee {get;set;}

        public virtual IngredientModel Ingredient { get; set; }

        [Required]
        [Display(Name = "Quantity of ingredient in coffee")]
        public int Quantity { get; set; }
    }
}
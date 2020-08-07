using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class IngredientInCoffeeModel
    {
        [Key]
        public Guid IngredientInCoffeeId { get; set; }

        public Guid CoffeeId { get; set; }

        public Guid IngredientId { get; set; }

        public virtual CoffeeModel Coffee {get;set;}

        public virtual IngredientModel Ingredient { get; set; }

        public int Quantity { get; set; }
    }
}
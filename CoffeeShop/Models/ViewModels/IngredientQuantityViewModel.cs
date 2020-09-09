namespace CoffeeShop.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class IngredientQuantityViewModel
    {
        public IngredientModel Ingredient { get; set; }

        [Display(Name = "Quantity in coffee")]
        public int QuantityInCoffee { get; set; }

    }
}
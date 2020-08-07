using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models.ViewModels
{
    public class CreateCoffeeViewModel
    {
        public Guid? CoffeeId { get; set; }

        [Required]
        public string Name { set; get; }

        public decimal BasePrice { get; set; }

        public decimal TotalPrice { get; set; }

        public string ImgUrl { set; get; }

        public string Description { get; set; }

        public int QuantityInStock { get; set; }

        public decimal IncomeCoef { get; set; }

        public List<string> selectedIngredients { set; get; }

        public List<int> selectedIngredientsQuantity { set; get; }

        public List<IngredientModel> availableIngredients { get; set; }

        public string Size { set; get; }

        public CreateCoffeeViewModel()
        {
            selectedIngredients = new List<string>();
            selectedIngredientsQuantity = new List<int>();
            availableIngredients = new List<IngredientModel>();
        }
    }
}
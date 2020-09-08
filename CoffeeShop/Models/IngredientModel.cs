namespace CoffeeShop.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class IngredientModel
    {
        [Key]
        public Guid IngredientId { get; set; }

        [Required]
        [Display(Name = "Ingredient name")]
        public String Name { get; set; }

        [Required]
        [Display(Name = "Ingredient price")]
        public decimal Price { get; set; }

        [Display(Name = "Ingredient image")]
        public String ImgUrl { get; set; }

        [Display(Name = "Ingredient description")]
        public String Description { get; set; }

        [JsonIgnore]
        public virtual List<CoffeeModel> Coffees { get; set; }

        [Display(Name = "Ingredients in stock")]
        public int QuantityInStock { get; set; }

        [Display(Name = "Ingredients used this week")]
        public int QuantityUsedLastWeek { get; set; }

        [Display(Name = "Total ingredients used")]
        public int TotalQuantityUsed { get; set; }
    }
}
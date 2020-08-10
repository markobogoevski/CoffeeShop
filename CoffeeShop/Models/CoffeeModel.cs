namespace CoffeeShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CoffeeModel
    {
        [Key]
        public Guid CoffeeId { get; set; }

        [Required]
        [Display(Name="Coffee name")]
        public String Name { get; set; }

        [Required]
        [Display(Name="Coffee bean price")]
        public decimal BasePrice { get; set; }

        [Display(Name = "Price")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Display(Name = "Coffee size")]
        public String  Size { get; set; }

        [Display(Name = "Coffee image")]
        public String ImgUrl { get; set; }

        [Display(Name = "Coffee description")]
        public String Description { get; set; }
        
        public virtual List<IngredientModel> Ingredients { get; set; }

        [Display(Name = "Quantity of coffee in stock")]
        public int QuantityInStock { get; set; }

        [Display(Name = "Quantity of coffee sold this week")]
        public int QuantitySoldLastWeek { get; set; }

        [Display(Name = "Total quantity of coffee sold throughout time")]
        public int TotalQuantitySold { get; set; }

        [Required]
        [Display(Name = "Coffee income coefficient")]
        public decimal IncomeCoef { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
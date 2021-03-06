﻿namespace CoffeeShop.Models
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

        public decimal ProductionPrice { get; set; }

        [Display(Name = "Coffee image")]
        public String ImgUrl { get; set; }

        [Display(Name = "Coffee description")]
        public String Description { get; set; }
        
        public virtual List<IngredientModel> Ingredients { get; set; }

        [Display(Name = "Coffee in stock")]
        public int QuantityInStock { get; set; }

        [Display(Name = "Coffee sold this week")]
        public int QuantitySoldLastWeek { get; set; }

        [Display(Name = "Total coffee sold")]
        public int TotalQuantitySold { get; set; }

        [Required]
        [Display(Name = "Coffee income coefficient")]
        public decimal IncomeCoef { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
namespace CoffeeShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CoffeeModel
    {
        [Key]
        public Guid CoffeeId { get; set; }

        public String Name { get; set; }

        public decimal BasePrice { get; set; }

        public decimal TotalPrice { get; set; }

        public String  Size { get; set; }

        public String ImgUrl { get; set; }

        public String Description { get; set; }
        
        public virtual List<IngredientModel> Ingredients { get; set; }

        public int QuantityInStock { get; set; }

        public int QuantitySoldLastWeek { get; set; }

        public int TotalQuantitySold { get; set; }

        public decimal IncomeCoef { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
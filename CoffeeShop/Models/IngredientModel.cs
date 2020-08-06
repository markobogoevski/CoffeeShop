using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class IngredientModel
    {
        [Key]
        public Guid IngredientId { get; set; }

        public String Name { get; set; }

        public decimal Price { get; set; }

        public String ImgUrl { get; set; }

        public String Description { get; set; }

        public virtual List<CoffeeModel> Coffees { get; set; }

        public int QuantityInStock { get; set; }

        public int QuantityUsedLastWeek { get; set; }

        public int TotalQuantityUsed { get; set; }

        public int QuantityInCoffee { get; set; }
    }
}
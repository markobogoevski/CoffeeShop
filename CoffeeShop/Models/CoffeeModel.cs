using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class CoffeeModel
    {
        [Key]
        public Guid CoffeeId { get; set; }

        public String Name { get; set; }

        public decimal Price { get; set; }

        public String  Size { get; set; }

        public String ImgUrl { get; set; }

        public String Description { get; set; }
        
        public virtual List<IngredientModel> Ingredients { get; set; }

    }
}
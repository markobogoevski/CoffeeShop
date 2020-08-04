using System;
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
    }
}
using CoffeeShop.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    // Component class from the Decorator pattern
    public abstract class CoffeeComponentModel
    {
        [Key]
        public Guid Id { get; set; }

        private string size;
        
        private string description;

        public string Size
        {
            get { return CoffeeSize.SMALL; }
            set { size = value; }
        }
        public string Description
        {
            get { return "Unknown Coffee"; }
            set { description = value; }
        }
        
        [Required]
        public String Name { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string ImgUrl { get; set; }

        public abstract double Cost();
    }
}
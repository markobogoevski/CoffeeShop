using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.CoffeeDecorator
{
    public class CoffeeConcrete : CoffeeComponentModel
    {
        CoffeeModel CoffeeModel { get; set; }

        public override decimal Cost()
        {
            return CoffeeModel.Price;
        }

        public override string Description()
        {
            return CoffeeModel.Description;
        }

        public override string Size()
        {
            return CoffeeModel.Size;
        }
    }
}
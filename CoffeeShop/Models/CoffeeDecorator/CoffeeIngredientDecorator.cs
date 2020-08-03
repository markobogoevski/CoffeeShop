using System;

namespace CoffeeShop.Models.CoffeeDecorator
{
    public abstract class CoffeeIngredientDecorator:CoffeeComponentModel
    {
        public abstract string GetDescription();

        public abstract string GetSize();
    }
}
using System;

namespace CoffeeShop.Models.CoffeeDecorator
{
    public abstract class CoffeeIngredientDecoratorModel:CoffeeComponentModel
    {
        public abstract string GetDescription();

        public abstract string GetSize();
    }
}
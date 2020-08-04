using CoffeeShop.Enumerations;
using System.Text;

namespace CoffeeShop.Models.CoffeeDecorator.Ingredients
{
    public class Mocha : CoffeeIngredientDecoratorModel
    {
        private CoffeeComponentModel _coffee;

        public Mocha(CoffeeComponentModel coffee)
        {
            _coffee = coffee;
        }

        public override double Cost()
        {
            double baseCost = _coffee.Cost();
            if (GetSize() == CoffeeSize.SMALL)
                baseCost += IngredientsCosts.Mocha * CoffeeSizeMultiplier.SmallMultipler;
            else if (GetSize() == CoffeeSize.MEDIUM)
                baseCost += IngredientsCosts.Mocha * CoffeeSizeMultiplier.MediumMultipler;
            else
                baseCost += IngredientsCosts.Mocha * CoffeeSizeMultiplier.BigMultipler;

            return baseCost;
        }

        public override string GetSize()
        {
            return _coffee.Size;
        }

        public override string GetDescription()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(_coffee.Description)
                         .Append(", ")
                         .Append(IngredientsDescriptions.Mocha);
            return stringBuilder.ToString();
        }
    }
}
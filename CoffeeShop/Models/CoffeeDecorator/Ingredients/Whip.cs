using CoffeeShop.Enumerations;
using System.Text;

namespace CoffeeShop.Models.CoffeeDecorator.Ingredients
{
    public class Whip : CoffeeIngredientDecoratorModel
    {
        private CoffeeComponentModel _coffee;

        public Whip(CoffeeComponentModel coffee)
        {
            _coffee = coffee;
        }

        public override double Cost()
        {
            double baseCost = _coffee.Cost();
            if (GetSize() == CoffeeSize.SMALL)
                baseCost += IngredientsCosts.Whip * CoffeeSizeMultiplier.SmallMultipler;
            else if (GetSize() == CoffeeSize.MEDIUM)
                baseCost += IngredientsCosts.Whip * CoffeeSizeMultiplier.MediumMultipler;
            else
                baseCost += IngredientsCosts.Whip * CoffeeSizeMultiplier.BigMultipler;

            return baseCost;
        }

        public override string GetDescription()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(_coffee.Description)
                         .Append(", ")
                         .Append(IngredientsDescriptions.Whip);
            return stringBuilder.ToString();
        }

        public override string GetSize()
        {
            return _coffee.Size;
        }
    }
}
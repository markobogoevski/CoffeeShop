using CoffeeShop.Enumerations;
using System.Text;

namespace CoffeeShop.Models.CoffeeDecorator.Ingredients
{
    public class Soy : CoffeeIngredientDecoratorModel
    {
        private CoffeeComponentModel _coffee;

        public Soy(CoffeeComponentModel coffee)
        {
            _coffee = coffee;
        }

        public override double Cost()
        {
            double baseCost = _coffee.Cost();
            if (GetSize() == CoffeeSize.SMALL)
                baseCost += IngredientsCosts.Soy * CoffeeSizeMultiplier.SmallMultipler;
            else if (GetSize() == CoffeeSize.MEDIUM)
                baseCost += IngredientsCosts.Soy * CoffeeSizeMultiplier.MediumMultipler;
            else
                baseCost += IngredientsCosts.Soy * CoffeeSizeMultiplier.BigMultipler;

            return baseCost;
        }

        public override string GetDescription()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(_coffee.Description)
                         .Append(", ")
                         .Append(IngredientsDescriptions.Soy);
            return stringBuilder.ToString();
        }

        public override string GetSize()
        {
            return _coffee.Size;
        }
    }
}
using CoffeeShop.Enumerations;
using System.Text;

namespace CoffeeShop.Models.CoffeeDecorator
{
    public class DecoratorConcrete : CoffeeComponentModel
    {

        private CoffeeComponentModel _coffee { get; set; }
        private IngredientModel _ingredient { get; set; }

        public DecoratorConcrete(CoffeeComponentModel coffee, IngredientModel ingredient)
        {
            _coffee = coffee;
            _ingredient = ingredient;
        }

        public override decimal Cost()
        {
            decimal baseCost = _coffee.Cost();
            if (_coffee.Size() == CoffeeSize.SMALL)
                baseCost += _ingredient.Price * CoffeeSizeMultiplier.SmallMultipler;
            else if (_coffee.Size() == CoffeeSize.MEDIUM)
                baseCost += _ingredient.Price * CoffeeSizeMultiplier.MediumMultipler;
            else
                baseCost += _ingredient.Price * CoffeeSizeMultiplier.BigMultipler;

            return baseCost;
        }

        public override string Description()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(_coffee.Description())
                         .Append(", ")
                         .Append(_ingredient.Description);
            return stringBuilder.ToString();
        }

        public override string Size()
        {
            return _coffee.Size();
        }
    }
}

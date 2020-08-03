using CoffeeShop.Enumerations;

namespace CoffeeShop.Models.CoffeeDecorator.CoffeeTypes
{
    public class Espresso : CoffeeComponentModel
    {
        public Espresso()
        {
            Description = CoffeeDescriptions.Espresso;
        }
        public override double Cost()
        {
            return CoffeeCosts.Espresso;
        }
    }
}
using CoffeeShop.Enumerations;

namespace CoffeeShop.Models.CoffeeDecorator.CoffeeTypes
{
    public class Nescafe : CoffeeComponentModel
    {
        public Nescafe()
        {
            Description = CoffeeDescriptions.Nescafe;
        }
        public override double Cost()
        {
            return CoffeeCosts.Nescafe;
        }
    }
}
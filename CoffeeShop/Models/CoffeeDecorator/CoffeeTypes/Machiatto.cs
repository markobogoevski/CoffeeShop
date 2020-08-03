using CoffeeShop.Enumerations;

namespace CoffeeShop.Models.CoffeeDecorator.CoffeeTypes
{
    public class Machiatto : CoffeeComponentModel
    {
        public Machiatto()
        {
            Description = CoffeeDescriptions.Machiatto;
        }
        public override double Cost()
        {
            return CoffeeCosts.Machiatto;
        }
    }
}
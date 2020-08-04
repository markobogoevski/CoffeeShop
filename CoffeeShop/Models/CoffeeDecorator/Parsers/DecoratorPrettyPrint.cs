using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeShop.Models.CoffeeDecorator.Parsers
{
    public class DecoratorPrettyPrint : CoffeeComponentModel
    {
        private CoffeeComponentModel _coffee;

        public DecoratorPrettyPrint(CoffeeComponentModel coffee)
        {
            _coffee = coffee;
        }

        public override decimal Cost()
        {
            return _coffee.Cost();
        }

        public override string Description()
        {
            StringBuilder stringBuilder = new StringBuilder();
            // Parsing description so far
            Dictionary<string, int> quantityDict = new Dictionary<string, int>();

            string[] parts = _coffee.Description().Split(',');
            string coffeeName = parts[0];
            string[] ingredients = parts.Skip(1).ToArray();

            foreach (string ingredient in ingredients)
            {
                if (quantityDict.ContainsKey(ingredient))
                {
                    int quantity = quantityDict[ingredient];
                    quantityDict[ingredient] = quantity + 1;
                }
                else
                {
                    quantityDict[ingredient] = 1;
                }
            }

            stringBuilder.Append("Serving coffee: ").Append(_coffee.Size())
                                                    .Append(coffeeName)
                                                    .Append(" with : ");
            foreach (string ingredient in quantityDict.Keys)
            {
                stringBuilder.Append(ingredient).Append(" x ").Append(quantityDict[ingredient].ToString())
                    .Append(", ");
            }
            stringBuilder.Append(". Enjoy your coffee!");
            return stringBuilder.ToString();
        }

        public override string Size()
        {
            return _coffee.Size();
        }
    }
}
namespace CoffeeShop.Models
{
    // Component class from the Decorator pattern
    public abstract class CoffeeComponentModel
    {
        public abstract decimal Cost();

        public abstract string Description();

        public abstract string Size();
    }
}
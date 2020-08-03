using CoffeeShop.Models;

namespace CoffeeShop.Services
{
    public class Repository
    {
        public static Repository repository;

        public static ApplicationDbContext db = null;

        // Singleton pattern for the repository
        private Repository() { }

        public static Repository GetInstance()
        {
            if (repository == null)
            {
                repository = new Repository();
            }

            db = new ApplicationDbContext();
            return repository;
        }

        // Methods here to communiciate with the database and implement logic 
    }
}
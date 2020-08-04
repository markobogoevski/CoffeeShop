using CoffeeShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShop.Services
{
    public class Repository
    {
        private static Repository _repository;

        private static ApplicationDbContext _db = null;

        // Singleton pattern for the repository
        private Repository() { }

        public static Repository GetInstance()
        {
            if (_repository == null)
            {
                _repository = new Repository();
            }

            _db = new ApplicationDbContext();
            return _repository;
        }

        public  List<CoffeeModel> GetAllCoffee()
        {
            var coffee = _db.Coffee.ToList();
            return coffee;
        }

        internal void Dispose()
        {
            _db.Dispose();
        }

    }
}
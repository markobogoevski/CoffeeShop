using CoffeeShop.Models;
using CoffeeShop.Models.Order;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public List<OrderModel> GetAllOrders()
        {
            var orders = _db.Orders.ToList();
            return orders;
        }

        internal object GetAllOrdersForUser(string userId)
        {
            var orders = _db.Orders.Where(order => order.User.Id == userId).ToList();
            return orders;
        }

        // Methods here to communiciate with the database and implement logic 
        public List<CoffeeModel> GetAllCoffee()
        {
            var coffee = _db.Coffee.ToList();
            return coffee;
        }

        public CoffeeModel GetCoffee(Guid? CoffeeId)
        {
            var coffee = _db.Coffee.Find(CoffeeId);
            if (coffee != null)
            {
                return coffee;
            }
            else
            {
                throw new Exception();
            }
        }

        public void EditCoffee(CoffeeModel coffeeModel)
        {
            _db.Entry(coffeeModel).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void RemoveCoffee(CoffeeModel coffeeModel)
        {
            _db.Coffee.Remove(coffeeModel);
            _db.SaveChanges();
        }

        internal void Dispose()
        {
            _db.Dispose();
        }
    }
}
using CoffeeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

        public List<IngredientModel> GetAllIngredients()
        {
            return _db.Ingredients.ToList(); 
        }

        public void CreateIngredient(IngredientModel _ingredient)
        {
            _db.Ingredients.Add(_ingredient);
        }

        public void UpdateIngredient(IngredientModel _ingredient)
        {
            var toChange = _db.Ingredients.Find(_ingredient.IngredientId);
            toChange.Name = _ingredient.Name;
            toChange.ImgUrl = _ingredient.ImgUrl;
            toChange.Price = _ingredient.Price;

            _db.SaveChanges();
        }

        public void DeleteIngredient(Guid ID)
        {
            IngredientModel toDelete = _db.Ingredients.Find(ID);

            if(toDelete != null)
            {
                _db.Ingredients.Remove(toDelete);
                _db.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
        }

        public List<IngredientModel> GetSortedIngredients(bool isAscending)
        {
            List<IngredientModel> allIngredients = GetAllIngredients();
            if(isAscending)
                return allIngredients.OrderBy(x=>x.Price).ToList();

            return allIngredients.OrderByDescending(x => x.Price).ToList();
        }

    }
}
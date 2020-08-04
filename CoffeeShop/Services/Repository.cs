using CoffeeShop.Enumerations;
using CoffeeShop.Models;
using CoffeeShop.Models.ViewModels;
using System;
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

        public CoffeeModel FindCoffee(Guid? coffeeId)
        {
            var coffee = _db.Coffee.Find(coffeeId);
            if (coffee == null)
            {
                throw new Exception();
            }

            return coffee;
        }

        public CoffeeModel DeleteCoffee(Guid? cofffeIdToDelete)
        {
            CoffeeModel coffeeToDelete = _db.Coffee.Find(cofffeIdToDelete);
            if (cofffeIdToDelete == null)
            {
                throw new Exception();
            }
            else
            {
                _db.Coffee.Remove(coffeeToDelete);
                _db.SaveChanges();
            }
            return coffeeToDelete;
        }

        public List<IngredientModel> GetIngredientsForCoffee(Guid? id)
        {
            CoffeeModel coffeeForIngredients = _db.Coffee.Find(id);
            if (coffeeForIngredients == null)
            {
                throw new Exception();
            }
            else
            {
                return _db.Ingredients.Where(ing => ing.Coffees.Any(coffee => coffee.CoffeeId == id))
                                  .ToList();

            }
        }

        internal void Dispose()
        {
            _db.Dispose();
        }

        public List<IngredientModel> GetIngredients()
        {
            return _db.Ingredients.ToList();
        }

        public void CreateCoffee(CreateCoffeeViewModel coffeeViewModel)
        {
            var coffeeIngredients = _db.Ingredients
                .Where(ing => coffeeViewModel.selectedIngredients.Select(inner_ing=> inner_ing.IngredientId.ToString())
                .Contains(ing.IngredientId.ToString()))
                .ToList();

            CoffeeModel newCoffee = new CoffeeModel
            {
                CoffeeId = Guid.NewGuid(),
                Name = coffeeViewModel.Name,
                Description = coffeeViewModel.Description,
                Ingredients = coffeeIngredients,
                Price = coffeeIngredients.Sum(ing => ing.Price * GetPriceMultiplierForSize(coffeeViewModel.Size)),
                ImgUrl = coffeeViewModel.ImgUrl,
                Size = coffeeViewModel.Size
            };

            _db.Coffee.Add(newCoffee);
            _db.SaveChanges();
        }

        public void EditCoffee(CreateCoffeeViewModel coffeeViewModel)
        {
            var coffee = _db.Coffee.Find(coffeeViewModel.CoffeeId);
            coffee.Name = coffeeViewModel.Name;
            coffee.Description = coffeeViewModel.Description;
            coffee.ImgUrl = coffeeViewModel.ImgUrl;
            coffee.Size = coffeeViewModel.Size;
            _db.Entry(coffee).Collection(c => c.Ingredients).Load();
            var newIngredients = _db.Ingredients.Where(ing => coffeeViewModel.selectedIngredients
                                                       .Select(inner_ing=>inner_ing.IngredientId.ToString())
                                                       .Contains(ing.IngredientId.ToString())).ToList();
            coffee.Ingredients = newIngredients;
            var newPrice = newIngredients.Sum(ing => ing.Price * GetPriceMultiplierForSize(coffeeViewModel.Size));
            coffee.Price = newPrice;
            _db.SaveChanges();
        }

        private decimal GetPriceMultiplierForSize(string size)
        {
            if (size == CoffeeSize.SMALL)
                return CoffeeSizeMultiplier.SmallMultipler;
            else if (size == CoffeeSize.MEDIUM)
                return CoffeeSizeMultiplier.MediumMultipler;
            else
                return CoffeeSizeMultiplier.BigMultipler;
        }

       
    }
}
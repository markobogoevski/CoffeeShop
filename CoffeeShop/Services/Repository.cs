using CoffeeShop.Enumerations;
using CoffeeShop.Models;
using CoffeeShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public List<IngredientModel> GetAllUsedIngredients()
        {
            var usedIngredients = _db.Ingredients.Where(ing => _db.Coffee.SelectMany(cof => cof.Ingredients)
                                                                         .Select(ing_in => ing_in.IngredientId)
                                                                         .Contains(ing.IngredientId)).ToList();
            return usedIngredients;
        }

        public List<CoffeeModel> GetCoffeeByIngredients(List<string> ids)
        {
            var coffee = _db.Coffee.Where(cof => cof.Ingredients.Select(ing => ing.IngredientId.ToString())
                                                                .Any(ing => ids.Contains(ing)))
                                                                .ToList();
            return coffee;
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

        public List<string> GetAllCoffeeTypes()
        {
            return _db.Coffee.Select(coff => coff.Name + " bean,  " + coff.BasePrice + " price").ToList();
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
                .Where(ing => coffeeViewModel.selectedIngredients
                .Contains(ing.IngredientId.ToString()))
                .ToList();

            CoffeeModel newCoffee = new CoffeeModel
            {
                CoffeeId = Guid.NewGuid(),
                Name = coffeeViewModel.Name,
                Description = coffeeViewModel.Description,
                Ingredients = coffeeIngredients,
                BasePrice = coffeeViewModel.BasePrice,
                TotalPrice = coffeeViewModel.BasePrice + coffeeIngredients.Sum(ing => ing.Price * GetPriceMultiplierForSize(coffeeViewModel.Size)),
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
            coffee.BasePrice = coffeeViewModel.BasePrice;
            _db.Entry(coffee).Collection(c => c.Ingredients).Load();
            var newIngredients = _db.Ingredients.Where(ing => coffeeViewModel.selectedIngredients
                                                       .Contains(ing.IngredientId.ToString())).ToList();
            coffee.Ingredients = newIngredients;
            var newPrice = newIngredients.Sum(ing => ing.Price * GetPriceMultiplierForSize(coffeeViewModel.Size))+coffee.BasePrice;
            coffee.TotalPrice = newPrice;
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

        public List<string> GetAllCoffeeSizes()
        {
            Type t = typeof(CoffeeSize);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            List<string> Sizes = new List<string>();
            foreach (FieldInfo fi in fields)
            {
                Sizes.Add(fi.GetValue(null).ToString());
            }
            return Sizes;
        }

        public CoffeeModel GetRandomCoffee()
        {
            var coffee = _db.Coffee.OrderBy(cof => Guid.NewGuid()).First();
            return coffee;
        }

        public CoffeeModel GetMostSoldCoffee()
        {
            var coffeeOrdered = _db.Coffee.OrderBy(cof => cof.TotalQuantitySold).ToList();
            return coffeeOrdered.Last();
        }

        public CoffeeModel GetLeastSoldCoffee()
        {
            var leastSold = _db.Coffee.OrderBy(cof => cof.TotalQuantitySold).First();
            return leastSold;
        }

        public CoffeeModel GetMostSoldCoffeeWeek()
        {
            var coffeeOrdered = _db.Coffee.OrderBy(cof => cof.QuantitySoldLastWeek).ToList();
            return coffeeOrdered.Last();
        }

        public CoffeeModel GetLeastSoldCoffeeWeek()
        {
            var leastSoldWeek = _db.Coffee.OrderBy(cof => cof.QuantitySoldLastWeek).First();
            return leastSoldWeek;
        }

        public decimal GetTotalProfitWeek(CoffeeModel coffee)
        {
            return coffee.TotalPrice * coffee.QuantitySoldLastWeek * coffee.IncomeCoef;
        }

        public decimal GetTotalProfit(CoffeeModel coffee)
        {
            return coffee.TotalPrice * coffee.TotalQuantitySold * coffee.IncomeCoef;
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
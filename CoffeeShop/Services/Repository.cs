using CoffeeShop.Controllers;
using CoffeeShop.Enumerations;
using CoffeeShop.Models;
using CoffeeShop.Models.Order;
using CoffeeShop.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Web.WebSockets;

namespace CoffeeShop.Services
{
    public class Repository
    {
        private static Repository _repository;

        private static ApplicationDbContext _db = null;
        private static UserManager<ApplicationUser> _userManager;

        // Singleton pattern for the repository
        private Repository() {
            
        }

        public List<OrderModel> GetOrders(string userId)
        {
            var orders = _db.Orders.ToList();

            if (_userManager.IsInRole(userId, UserRoles.User))
                return orders.Where(ord => ord.User!=null && ord.User.Id == userId).ToList();

            return orders;
        }

        public List<CancelOrderViewModel> GetOrderCancels(List<OrderModel> orders, string userId)
        {
            List<CancelOrderViewModel> cancels = new List<CancelOrderViewModel>();
            foreach(var order in orders)
            {
                cancels.Add(new CancelOrderViewModel
                {
                    OrderId = order.OrderId,
                    Cancellable = GetCancelOrderOption(order, userId)
                });
            }
            return cancels;
        }

        private bool GetCancelOrderOption(OrderModel order, string userId)
        {
            if (order.OrderStatus == OrderStatus.INACTIVE)
                return true;

            var topFiveOrders = _db.Orders.Where(ord => ord.OrderStatus == OrderStatus.PENDING)
                                          .OrderBy(ord => ord.OrderTime)
                                          .Take(5)
                                          .ToList();

            if (!topFiveOrders.Contains(order))
                return true;

            return false;
        }

        public static Repository GetInstance()
        {
            if (_repository == null)
            {
                _repository = new Repository();
            }

            _db = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(_db);
            _userManager = new UserManager<ApplicationUser>(userStore);
            return _repository;
        }

        public List<IngredientModel> GetAllUsedIngredients(string userId)
        {
            if (_userManager.IsInRole(userId, UserRoles.User))
            {
                return _db.Ingredients.Where(ing => _db.Coffee.Where(cof => cof.User == null || cof.User.Id == userId)
                                                                         .SelectMany(cof => cof.Ingredients)
                                                                         .Select(ing_in => ing_in.IngredientId)
                                                                         .Contains(ing.IngredientId)).ToList();
            }
          
            return _db.Ingredients.Where(ing => _db.Coffee.SelectMany(cof => cof.Ingredients)
                                                            .Select(ing_in => ing_in.IngredientId)
                                                            .Contains(ing.IngredientId)).ToList();
        }

        public IngredientModel GetIngredient(Guid? id)
        {
            var ingredient = _db.Ingredients.Find(id.Value);
            return ingredient;
        }

        public OrderItemModel GetOrderItem(Guid orderItemId)
        {
            var orderItem = _db.OrderItems.Find(orderItemId);
            return orderItem;
        }

        public void DeactivateOrder(Guid? id)
        {
            var order = _db.Orders.Find(id.Value);
            order.OrderStatus = OrderStatus.INACTIVE;
            order.OrderTime = DateTime.MaxValue;
            _db.SaveChanges();
        }

        public void DeleteOrder(Guid? id)
        {
            var order = _db.Orders.Find(id.Value);
            _db.OrderItems.RemoveRange(order.OrderItems);
            _db.Orders.Remove(order);
            _db.SaveChanges();
        }

        public void RateOrder(string orderId, string grade)
        {
            var order = _db.Orders.Find(Guid.Parse(orderId));
            order.OrderRating = Convert.ToInt32(grade);
            _db.SaveChanges();
        }

        public void FinishOrder(Guid? id)
        {
            var order = _db.Orders.Find(id.Value);
            order.OrderStatus = OrderStatus.FINISHED;
            order.OrderFinishTime = DateTime.Now;
            foreach(var orderItem in order.OrderItems)
            {
                var coffee = FindCoffee(orderItem.Coffee.CoffeeId);
                coffee.TotalQuantitySold += orderItem.Quantity;
                var ingredientsInCoffee = GetIngredientsInCoffee(coffee.CoffeeId);
                foreach(var ingredientInCoffee in ingredientsInCoffee)
                {
                    var ing = FindIngredient(ingredientInCoffee.IngredientId);
                    ing.TotalQuantityUsed += ingredientInCoffee.Quantity;
                }              
            }
            _db.SaveChanges();
        }

        public void ForceCancelOrder(Guid? id)
        {
            var order = _db.Orders.Find(id.Value);
            order.OrderStatus = OrderStatus.CANCELLED;
            _db.SaveChanges();
        }

        public void DiscardOrder(Guid? id)
        {
            var order = _db.Orders.Find(id.Value);
            order.User = null;
            _db.SaveChanges();
        }

        public IngredientModel FindIngredient(Guid? ingredientId)
        {
            var ingredient = _db.Ingredients.Find(ingredientId.Value);
            return ingredient;
        }

        public void ActivateOrder(Guid? id)
        {
            var order = _db.Orders.Find(id.Value);
            order.OrderStatus = OrderStatus.PENDING;
            order.OrderTime = DateTime.Now;
            _db.SaveChanges();
        }

        public List<CoffeeModel> GetCoffeeByIngredients(List<string> ids, string userId)
        {
            var coffee = _db.Coffee.Where(cof => cof.Ingredients.Select(ing => ing.IngredientId.ToString())
                                                                .Any(ing => ids.Contains(ing)))
                                                                .ToList();
            if (userId!=null && _userManager.IsInRole(userId, UserRoles.User))
                return coffee.Where(cof => cof.User == null || cof.User.Id == userId).ToList();

            return coffee;
        }

        public void CreateOrder(List<OrderItemModel> orderItems, string userId, string address)
        {
            ApplicationUser user = _db.Users.Find(userId);
            OrderModel newOrder = new OrderModel
            {
                OrderId = Guid.NewGuid(),
                Address = address,
                OrderStatus = OrderStatus.INACTIVE,
                User = user,
                OrderTime = DateTime.Now,
                OrderFinishTime = DateTime.Now,
                OrderRating = 0
            };

            foreach(var item in orderItems)
            {
                var newOrderItem = new OrderItemModel
                {
                    OrderItemId = Guid.NewGuid(),
                    Coffee = _db.Coffee.Find(item.Coffee.CoffeeId),
                    Quantity = item.Quantity,
                    Order = newOrder
                };
                _db.OrderItems.Add(newOrderItem);
                newOrder.OrderItems.Add(newOrderItem);
            }

            _db.Orders.Add(newOrder);
            _db.SaveChanges();
        }

        public CoffeeModel GetCoffee(string id)
        {
            var coffee = _db.Coffee.Find(Guid.Parse(id));
            return coffee;
        }

        public  List<CoffeeModel> GetAllCoffee(string userId)
        {
            var coffee = _db.Coffee.ToList();

            if (_userManager.IsInRole(userId, UserRoles.User))
                return coffee.Where(cof => cof.User == null || cof.User.Id == userId).ToList();

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

        public void AddOrderItem(OrderItemModel orderItemModel)
        {
            _db.OrderItems.Add(orderItemModel);
            _db.SaveChanges();
            return;
        }

        public void UpdateIngredientStock(string id, string quantity)
        {
            var ingredient = _db.Ingredients.Find(Guid.Parse(id));
            ingredient.QuantityInStock += Convert.ToInt32(quantity);
            _db.SaveChanges();
        }

        public IngredientModel GetMostUsedIngredientWeek()
        {
            var ingredientsOrdered = _db.Ingredients.OrderBy(ing => ing.QuantityUsedLastWeek).ToList();
            return ingredientsOrdered.Last();
        }

        public IngredientModel GetLeastUsedIngredientWeek()
        {
            return _db.Ingredients.OrderBy(ing => ing.QuantityUsedLastWeek).First();
        }

        public IngredientModel GetLeastUsedIngredient()
        {
            return _db.Ingredients.OrderBy(ing => ing.TotalQuantityUsed).First();
        }

        public IngredientModel GetMostUsedIngredient()
        {
            var ingredientsOrdered = _db.Ingredients.OrderBy(ing => ing.TotalQuantityUsed).ToList();
            return ingredientsOrdered.Last();
        }

        public List<IngredientInCoffeeModel> GetIngredientsInCoffee(Guid coffeeId)
        {
            var coffee = _db.Coffee.Find(coffeeId);
            if (coffee == null)
            {
                throw new Exception();
            }

            var ingredientsInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffee.CoffeeId).ToList();
            return ingredientsInCoffee;
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
                var previousIngredientInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffeeToDelete.CoffeeId).ToList();

                // Readding quantity to ingredients
                for (int i = 0; i < previousIngredientInCoffee.Count; i++)
                {
                    var ingredientId = previousIngredientInCoffee.ElementAt(i).IngredientId;
                    var ingredient = _db.Ingredients.Find(ingredientId);
                    var quantity = previousIngredientInCoffee.ElementAt(i).Quantity;
                    ingredient.QuantityInStock += quantity*coffeeToDelete.QuantityInStock;
                }

                // Deleting relations
                var toDelete = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffeeToDelete.CoffeeId).ToList();
                _db.IngredientInCoffee.RemoveRange(toDelete);
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

        public void CreateCoffee(CreateCoffeeViewModel coffeeViewModel, string userId)
        {
            var coffeeIngredients = _db.Ingredients
                .Where(ing => coffeeViewModel.selectedIngredients
                .Contains(ing.IngredientId.ToString()))
                .ToList();

            var ingredientPrice = GetIngredientPrice(coffeeViewModel);

            CoffeeModel newCoffee = new CoffeeModel
            {
                CoffeeId = Guid.NewGuid(),
                Name = coffeeViewModel.Name,
                Description = coffeeViewModel.Description,
                Ingredients = coffeeIngredients,
                BasePrice = coffeeViewModel.BasePrice,
                TotalPrice = coffeeViewModel.BasePrice + ingredientPrice,
                ImgUrl = coffeeViewModel.ImgUrl,
                Size = coffeeViewModel.Size,
                QuantityInStock = 1,
                TotalQuantitySold = 0,
                QuantitySoldLastWeek = 0,
                IncomeCoef = coffeeViewModel.IncomeCoef
            };

            if (userId != null)
            {
                newCoffee.User = _db.Users.Find(userId);
            }

            _db.Coffee.Add(newCoffee);

            for (int i = 0; i < coffeeIngredients.Count; i++)
            {
                var ingredientId = Guid.Parse(coffeeViewModel.selectedIngredients.ElementAt(i));
                var ingredient = _db.Ingredients.Find(ingredientId);
                var quantity = coffeeViewModel.selectedIngredientsQuantity.ElementAt(i);
                ingredient.QuantityInStock -= quantity;

                IngredientInCoffeeModel newIngredientInCoffeeModel = new IngredientInCoffeeModel
                {
                    IngredientInCoffeeId = Guid.NewGuid(),
                    CoffeeId = newCoffee.CoffeeId,
                    IngredientId = ingredientId,
                    Quantity = quantity,
                    Coffee = newCoffee,
                    Ingredient = ingredient
                };
                _db.IngredientInCoffee.Add(newIngredientInCoffeeModel);
            }

            

            _db.SaveChanges();
        }

        public int GetMaxQuantityForCoffee(Guid id)
        {
            List<int> quantitiesInCoffee = new List<int>();
            List<int> quantitiesInStock = new List<int>();
            var ingredientsInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == id)
                                                            .ToList();
            foreach(var ingredientInCoffee in ingredientsInCoffee)
            {
                var ingredientId = ingredientInCoffee.IngredientId;
                quantitiesInCoffee.Add(ingredientInCoffee.Quantity);
                quantitiesInStock.Add(_db.Ingredients.Find(ingredientId).QuantityInStock);
            }

            List<int> moduls = new List<int>();
            for (int i = 0; i < quantitiesInCoffee.Count; i++)
            {
                moduls.Add(quantitiesInStock.ElementAt(i) / quantitiesInCoffee.ElementAt(i));
            }

            if (moduls.Count != 0)
            {
                return moduls.Min();
            }
            else
            {
                return 500;
            }
        }

        public List<int> GetSelectedIngredientQuantitiesForCoffee(Guid? id, List<string> selectedIngredients)
        {
            // Needed to ensure correct order

            List<int> quantities = new List<int>();
            foreach(string ingredientId in selectedIngredients)
            {
                Guid ingId = Guid.Parse(ingredientId);
                quantities.Add(_db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == id.Value && ingc.IngredientId == ingId)
                                                     .Select(ingc => ingc.Quantity)
                                                     .First());
            }
            return quantities;
        }

        private decimal GetIngredientPrice(CreateCoffeeViewModel coffeeViewModel)
        {
            decimal ingredientPrice = 0;
            for (int i = 0; i < coffeeViewModel.selectedIngredients.Count; i++)
            {
                var ingredient = _db.Ingredients.Find(Guid.Parse(coffeeViewModel.selectedIngredients.ElementAt(i)));
                ingredientPrice += ingredient.Price * GetPriceMultiplierForSize(coffeeViewModel.Size) 
                                                    * coffeeViewModel.selectedIngredientsQuantity.ElementAt(i);
            }
            return ingredientPrice;
        }

        public List<IngredientModel> GetAvailableIngredients()
        {
            return _db.Ingredients.Where(ing => ing.QuantityInStock >= 1).ToList();
        }

        public void EditCoffee(CreateCoffeeViewModel coffeeViewModel)
        {
            var coffee = _db.Coffee.Find(coffeeViewModel.CoffeeId);
            coffee.Name = coffeeViewModel.Name;
            coffee.Description = coffeeViewModel.Description;
            coffee.IncomeCoef = coffeeViewModel.IncomeCoef;
            coffee.QuantityInStock = coffeeViewModel.QuantityInStock;
            coffee.ImgUrl = coffeeViewModel.ImgUrl;
            coffee.Size = coffeeViewModel.Size;
            coffee.BasePrice = coffeeViewModel.BasePrice;
            _db.Entry(coffee).Collection(c => c.Ingredients).Load();
            var newIngredients = _db.Ingredients.Where(ing => coffeeViewModel.selectedIngredients
                                                       .Contains(ing.IngredientId.ToString())).ToList();
            var previousIngredientInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffee.CoffeeId).ToList();

            // Readding quantity to ingredients
            for (int i = 0; i < previousIngredientInCoffee.Count; i++)
            {
                var ingredientId = previousIngredientInCoffee.ElementAt(i).IngredientId;
                var ingredient = _db.Ingredients.Find(ingredientId);
                var quantity = previousIngredientInCoffee.ElementAt(i).Quantity;
                ingredient.QuantityInStock += quantity;
            }

            coffee.Ingredients = newIngredients;
            var ingredientPrice = GetIngredientPrice(coffeeViewModel);
            coffee.TotalPrice = coffeeViewModel.BasePrice+ingredientPrice;

            // Deleting relations
            var toDelete = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffeeViewModel.CoffeeId).ToList();
            _db.IngredientInCoffee.RemoveRange(toDelete);

            // Remaking relations
            for (int i = 0; i < newIngredients.Count; i++)
            {
                var ingredientId = Guid.Parse(coffeeViewModel.selectedIngredients.ElementAt(i));
                var ingredient = _db.Ingredients.Find(ingredientId);
                var quantity = coffeeViewModel.selectedIngredientsQuantity.ElementAt(i);
                ingredient.QuantityInStock -= quantity;

            IngredientInCoffeeModel newIngredientInCoffeeModel = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = Guid.NewGuid(),
                    CoffeeId = coffee.CoffeeId,
                    IngredientId = ingredientId,
                    Quantity = quantity,
                    Coffee = coffee,
                    Ingredient = ingredient
                };
                _db.IngredientInCoffee.Add(newIngredientInCoffeeModel);
            }

            _db.SaveChanges();
        }

        public void UpdateCoffeeStock(string id, string quantity)
        {
            var coffee = _db.Coffee.Find(Guid.Parse(id));
            var ingredientsInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffee.CoffeeId).ToList();
            for (int i = 0; i < ingredientsInCoffee.Count; i++)
            {
                var ingredientId = ingredientsInCoffee.ElementAt(i).IngredientId;
                var ingredient = _db.Ingredients.Find(ingredientId);
                var ingQuantity = ingredientsInCoffee.ElementAt(i).Quantity;
                ingredient.QuantityInStock -= ingQuantity*Convert.ToInt32(quantity);
            }
            coffee.QuantityInStock += Convert.ToInt32(quantity);
            if (coffee.QuantityInStock == 0)
            {
                DeleteCoffee(coffee.CoffeeId);
            }

            _db.SaveChanges();
        }

        public List<CoffeeStatisticViewModel> GetCoffeeStatistics(List<CoffeeModel> coffee)
        {
            List<CoffeeStatisticViewModel> coffeeStatistics = new List<CoffeeStatisticViewModel>();
            foreach(var coffeeUnit in coffee)
            {
                coffeeStatistics.Add(new CoffeeStatisticViewModel
                {
                    Coffee = coffeeUnit,
                    MaxIncreaseStock = GetMaxQuantityForCoffee(coffeeUnit.CoffeeId)
                });
            }
            return coffeeStatistics;
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

        public List<string> GetAllOrderStatuses()
        {
            Type t = typeof(OrderStatus);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            List<string> Statuses = new List<string>();
            foreach (FieldInfo fi in fields)
            {
                Statuses.Add(fi.GetValue(null).ToString());
            }
            return Statuses;
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
            _ingredient.IngredientId = Guid.NewGuid();
            _ingredient.QuantityInStock = IngredientStock.DEFAULT_STOCK_QUANTITY;
            _db.Ingredients.Add(_ingredient);
            _db.SaveChanges();
        }

        public void UpdateIngredient(IngredientModel _ingredient)
        {
            var toChange = _db.Ingredients.Find(_ingredient.IngredientId);
            toChange.Name = _ingredient.Name;
            toChange.ImgUrl = _ingredient.ImgUrl;
            toChange.Price = _ingredient.Price;
            toChange.Description = _ingredient.Description;
            _db.SaveChanges();
        }

        public void DeleteIngredient(Guid ID)
        {
            IngredientModel toDelete = _db.Ingredients.Find(ID);

            if(toDelete != null)
            {
                var coffee = GetCoffeeByIngredients(new List<string>() { ID.ToString() }, null);
                foreach(var coffeeItem in coffee)
                {
                    DeleteCoffee(coffeeItem.CoffeeId);
                }
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
            List<IngredientModel> allIngredients = _repository.GetAllIngredients();
            if(isAscending)
                return allIngredients.OrderBy(x=>x.Price).ToList();

            return allIngredients.OrderByDescending(x => x.Price).ToList();
        }

        private List<IngredientModel> GetAllIngredients()
        {
            return _db.Ingredients.ToList();
        }
    }
}
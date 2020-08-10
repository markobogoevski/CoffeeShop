namespace CoffeeShop.Services
{
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

    public class Repository
    {
        private static Repository _repository;

        private static ApplicationDbContext _db = null;
        private static UserManager<ApplicationUser> _userManager;

        private Repository() {
            
        }

        // Singleton pattern to hold only one instance of the Repository in memory 
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

        #region Coffee
        // Gets all coffee for a user. Admin/owner user roles have access to all coffee (user-made and admin/owner-made)
        public IEnumerable<CoffeeModel> GetAllCoffeeForUser(string userId)
        {
            var coffee = _db.Coffee.ToList();

            if (userId == null || _userManager.IsInRole(userId, UserRoles.User))
            {
                coffee = coffee.Where(cof => cof.User == null || cof.User.Id == userId).ToList();
            }

            if (coffee != null)
            {
                return coffee;
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets coffee by id from database
        public CoffeeModel FindCoffee(Guid? coffeeId)
        {
            var coffee = _db.Coffee.Find(coffeeId);
            if (coffee != null)
            {
                return coffee;
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets coffee for a specific user according to a list of ingredients used. 
        // Admin/owner user roles have access to all coffee (user-made and admin/owner-made).
        public IEnumerable<CoffeeModel> GetCoffeeForUserByIngredients(List<string> ingredientIds, string userId)
        {
            var coffee = _db.Coffee.Where(cof => cof.Ingredients.Select(ing => ing.IngredientId.ToString())
                                                                .Any(ing => ingredientIds.Contains(ing)))
                                                                .ToList();

            if (userId != null && _userManager.IsInRole(userId, UserRoles.User))
            {
                coffee = coffee.Where(cof => cof.User == null || cof.User.Id == userId).ToList();
            }

            if (coffee != null)
            {
                return coffee;
            }
            else
            {
                throw new Exception();
            }
        }

        // Creates a coffee according for a user to a view model. If the userId is null, the coffee is created by
        // an admin or owner and can be seen by all users.
        public void CreateCoffee(CreateCoffeeViewModel coffeeViewModel, string userId)
        {
            // Get the coffee ingredients
            var coffeeIngredients = _db.Ingredients
                .Where(ing => coffeeViewModel.selectedIngredients
                .Contains(ing.IngredientId.ToString()))
                .ToList();

            if (coffeeIngredients == null)
            {
                throw new Exception();
            }

            // Get the total price of the coffee
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
                // Add the coffee to a user if the coffee is custom made (userId is null)
                newCoffee.User = _db.Users.Find(userId);
            }

            _db.Coffee.Add(newCoffee);

            for (int i = 0; i < coffeeIngredients.Count; i++)
            {
                var ingredientId = Guid.Parse(coffeeViewModel.selectedIngredients.ElementAt(i));
                try
                {
                    // Update the stock quantity of used ingredients
                    var ingredient = FindIngredient(ingredientId);
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
                    // Add the ingredient in coffee relation for each ingredient to keep track of quantities of 
                    // ingredients used in the coffee
                    _db.IngredientInCoffee.Add(newIngredientInCoffeeModel);
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
            _db.SaveChanges();
        }

        // Edits a coffee according to a view model. Only possible to do if role is admin/owner.
        public void EditCoffee(CreateCoffeeViewModel coffeeViewModel)
        {
            try
            {
                // Update the attributes of the coffee in the database
                var coffee = FindCoffee(coffeeViewModel.CoffeeId);
                coffee.Name = coffeeViewModel.Name;
                coffee.Description = coffeeViewModel.Description;
                coffee.IncomeCoef = coffeeViewModel.IncomeCoef;
                coffee.QuantityInStock = coffeeViewModel.QuantityInStock;
                coffee.ImgUrl = coffeeViewModel.ImgUrl;
                coffee.Size = coffeeViewModel.Size;
                coffee.BasePrice = coffeeViewModel.BasePrice;
                _db.Entry(coffee).Collection(c => c.Ingredients).Load();
                // Get the new ingredients selected for the coffee
                var newIngredients = _db.Ingredients.Where(ing => coffeeViewModel.selectedIngredients
                                                           .Contains(ing.IngredientId.ToString())).ToList();
                // Get the previous ingredients to update their stock quantities and relations later
                var previousIngredientInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffee.CoffeeId).ToList();
                if (newIngredients == null || previousIngredientInCoffee == null)
                {
                    throw new Exception();
                }

                // Readding quantity to previous ingredients which are not used anymore in the coffee
                for (int i = 0; i < previousIngredientInCoffee.Count; i++)
                {
                    var ingredientId = previousIngredientInCoffee.ElementAt(i).IngredientId;
                    var ingredient = _db.Ingredients.Find(ingredientId);
                    var quantity = previousIngredientInCoffee.ElementAt(i).Quantity;
                    ingredient.QuantityInStock += quantity;
                }

                coffee.Ingredients = newIngredients;
                try
                {
                    // Calculating the new total price of the coffee
                    var ingredientPrice = GetIngredientPrice(coffeeViewModel);
                    coffee.TotalPrice = coffeeViewModel.BasePrice + ingredientPrice;
                }
                catch (Exception)
                {
                    throw new Exception();
                }

                // Deleting ingredient in coffee relations for the edited coffee
                var toDelete = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffeeViewModel.CoffeeId).ToList();
                if (toDelete == null)
                {
                    throw new Exception();
                }
                _db.IngredientInCoffee.RemoveRange(toDelete);

                // Recreating the ingredient in coffee relations for the new ingredients and edited coffee
                for (int i = 0; i < newIngredients.Count; i++)
                {
                    var ingredientId = Guid.Parse(coffeeViewModel.selectedIngredients.ElementAt(i));
                    try
                    {
                        var ingredient = FindIngredient(ingredientId);
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
                    catch (Exception)
                    {
                        throw new Exception();
                    }
                }
                _db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Deletes a coffee provided an id. Only possible for admin/owner
        public void DeleteCoffee(Guid? coffeeId)
        {
            try
            {
                var coffeeToDelete = FindCoffee(coffeeId);
                var previousIngredientInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffeeToDelete.CoffeeId).ToList();
                if (previousIngredientInCoffee != null)
                {
                    // Readding quantity in stock to ingredients which were used for the coffee

                    for (int i = 0; i < previousIngredientInCoffee.Count; i++)
                    {
                        var ingredientId = previousIngredientInCoffee.ElementAt(i).IngredientId;
                        var ingredient = _db.Ingredients.Find(ingredientId);
                        var quantity = previousIngredientInCoffee.ElementAt(i).Quantity;
                        ingredient.QuantityInStock += quantity * coffeeToDelete.QuantityInStock;
                    }

                    // Deleting ingredient in coffee relations

                    var toDelete = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffeeToDelete.CoffeeId).ToList();
                    _db.IngredientInCoffee.RemoveRange(toDelete);
                    _db.Coffee.Remove(coffeeToDelete);
                    _db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Updates the coffee quantity in stock 
        public void UpdateCoffeeStock(Guid? coffeeId, string quantity)
        {
            try
            {
                var coffee = FindCoffee(coffeeId);
                var ingredientsInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffee.CoffeeId).ToList();
                if (ingredientsInCoffee == null)
                    throw new Exception();

                // After getting all the used ingredients, updating their stock quantity also according to the
                // updated coffee quantity in stock

                for (int i = 0; i < ingredientsInCoffee.Count; i++)
                {
                    var ingredientId = ingredientsInCoffee.ElementAt(i).IngredientId;
                    var ingredient = _db.Ingredients.Find(ingredientId);
                    var ingQuantity = ingredientsInCoffee.ElementAt(i).Quantity;
                    ingredient.QuantityInStock -= ingQuantity * Convert.ToInt32(quantity);
                }
                coffee.QuantityInStock += Convert.ToInt32(quantity);
                if (coffee.QuantityInStock == 0)
                {
                    try
                    {
                        DeleteCoffee(coffee.CoffeeId);
                    }
                    catch (Exception)
                    {
                        throw new Exception();
                    }
                }

                _db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Gets the maximum number of possible increase regarding the quantity in stock attribute for the coffee
        public int GetMaxQuantityForCoffee(Guid? coffeeId)
        {
            // Keeping lists of quantities of ingredients used in the coffee and their respective quantities in stock
            List<int> quantitiesInCoffee = new List<int>();
            List<int> quantitiesInStock = new List<int>();
            try
            {
                // Get the relations for ingredients in coffee to get quantities
                var ingredientsInCoffee = GetIngredientsInCoffee(coffeeId);
                foreach (var ingredientInCoffee in ingredientsInCoffee)
                {
                    var ingredientId = ingredientInCoffee.IngredientId;
                    quantitiesInCoffee.Add(ingredientInCoffee.Quantity);
                    try
                    {
                        var ingredient = FindIngredient(ingredientId);
                        quantitiesInStock.Add(ingredient.QuantityInStock);
                    }
                    catch (Exception)
                    {
                        throw new Exception();
                    }
                }

                // Getting the minimum number out of each division(quantity in stock over quantity in coffee)
                List<int> divisions = new List<int>();
                for (int i = 0; i < quantitiesInCoffee.Count; i++)
                {
                    divisions.Add(quantitiesInStock.ElementAt(i) / quantitiesInCoffee.ElementAt(i));
                }

                if (divisions.Count != 0)
                {
                    return divisions.Min();
                }
                else
                {
                    // Default number of non-ingredients coffee which we can make
                    return 500;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }    
        }

        // Gets all coffee sizes using Reflection. 
        public IEnumerable<string> GetAllCoffeeSizes()
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

        // Gets a random coffee from all coffees in database which are not custom made
        public CoffeeModel GetRandomCoffee()
        {
            var coffee = _db.Coffee.Where(cof=>cof.User==null)
                                   .OrderBy(cof => Guid.NewGuid())
                                   .FirstOrDefault();
            if (coffee != null)
            {
                return coffee;
            }
            else
            {
                throw new Exception();
            }
        }

        public CoffeeModel GetCoffeeDay()
        {
            try
            {
                var randomCoffee = GetRandomCoffee();
                randomCoffee.TotalPrice *= 0.7m;
                return randomCoffee;
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Gets the overall most sold coffee which exists in database which is not custom made
        public CoffeeModel GetMostSoldCoffee()
        {
            var coffeeOrdered = _db.Coffee.Where(cof => cof.User == null)
                                          .OrderBy(cof => cof.TotalQuantitySold)
                                          .ToList();
            if (coffeeOrdered != null)
            {
                return coffeeOrdered.Last();
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets the overall least sold coffee which exists in database which is not custom made
        public CoffeeModel GetLeastSoldCoffee()
        {
            var leastSold = _db.Coffee.Where(cof=>cof.User == null)
                                      .OrderBy(cof => cof.TotalQuantitySold)
                                      .First();
            if (leastSold != null)
            {
                return leastSold;
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets the most sold coffee this week which exists in database and is not custom made
        public CoffeeModel GetMostSoldCoffeeWeek()
        {
            var coffeeOrdered = _db.Coffee.Where(cof=>cof.User == null)
                                          .OrderBy(cof => cof.QuantitySoldLastWeek)
                                          .ToList();
            if (coffeeOrdered != null)
            {
                return coffeeOrdered.Last();
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets the least sold coffee this week which exists in database and is not custom made
        public CoffeeModel GetLeastSoldCoffeeWeek()
        {
            var leastSoldWeek = _db.Coffee.Where(cof=>cof.User == null)
                                          .OrderBy(cof => cof.QuantitySoldLastWeek)
                                          .First();
            if (leastSoldWeek != null)
            {
                return leastSoldWeek;
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets the profit made this week from the coffee provided according to its income coefficient
        public decimal GetTotalProfitWeekCoffee(CoffeeModel coffee)
        {
            return coffee.TotalPrice * coffee.QuantitySoldLastWeek * coffee.IncomeCoef;
        }

        // Gets the total profit made from the coffee provided according to its income coefficient
        public decimal GetTotalProfitCoffee(CoffeeModel coffee)
        {
            return coffee.TotalPrice * coffee.TotalQuantitySold * coffee.IncomeCoef;
        }

        // Gets statistics for the provided coffee
        public IEnumerable<CoffeeStatisticViewModel> GetCoffeeStatistics(IEnumerable<CoffeeModel> coffee)
        {
            List<CoffeeStatisticViewModel> coffeeStatistics = new List<CoffeeStatisticViewModel>();
            foreach (var coffeeUnit in coffee)
            {
                coffeeStatistics.Add(new CoffeeStatisticViewModel
                {
                    Coffee = coffeeUnit,
                    MaxIncreaseStock = GetMaxQuantityForCoffee(coffeeUnit.CoffeeId)
                });
            }
            return coffeeStatistics;
        }

        // Returns price multipler for coffee size (used for price for ingredients in coffee)
        private decimal GetPriceMultiplierForCoffeeSize(string coffeeSize)
        {
            if (coffeeSize == CoffeeSize.SMALL)
                return CoffeeSizeMultiplier.SmallMultipler;
            else if (coffeeSize == CoffeeSize.MEDIUM)
                return CoffeeSizeMultiplier.MediumMultipler;
            else
                return CoffeeSizeMultiplier.BigMultipler;
        }
        #endregion

        #region Ingredient

        // Gets all ingredients from database
        public IEnumerable<IngredientModel> GetIngredients()
        {
            var ingredients = _db.Ingredients.ToList();
            if (ingredients != null)
            {
                return ingredients;
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets an ingredient according to its id from database
        public IngredientModel FindIngredient(Guid? ingredientId)
        {
            var ingredient = _db.Ingredients.Find(ingredientId);
            if (ingredient != null)
            {
                return ingredient;
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets every used ingredient for a certain coffee (without quantity of ingredients)
        public IEnumerable<IngredientModel> GetIngredientsForCoffee(Guid? coffeeId)
        {
            try
            {
                var coffee = FindCoffee(coffeeId);
                var ingredients = _db.Ingredients.Where(ing => ing.Coffees.Any(cof => cof.CoffeeId == coffeeId))
                                  .ToList();
                if (ingredients != null)
                {
                    return ingredients;
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Gets all relations between the ingredients used in the provided coffee. This returns each ingredient
        // with its quantity used in the coffee provided.
        public IEnumerable<IngredientInCoffeeModel> GetIngredientsInCoffee(Guid? coffeeId)
        {
            try
            {
                var coffee = FindCoffee(coffeeId);
                var ingredientsInCoffee = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffee.CoffeeId).ToList();
                if (ingredientsInCoffee != null)
                {
                    return ingredientsInCoffee;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Gets all ingredients with at least >=1 stock quantity. Note: Ingredients with 0 quantity are still kept
        // in the database because users can't order ingredients and each ingredient's stock quantity can be changed.
        public IEnumerable<IngredientModel> GetAvailableIngredients(Guid? coffeeId)
        {
            var ingredients = _db.Ingredients.Where(ing => ing.QuantityInStock >= 1).ToList();

            if (coffeeId.HasValue)
            {
                var previousIngredients = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == coffeeId)
                                                                .Select(ingc => ingc.Ingredient)
                                                                .ToList();
                foreach(var ingredient in previousIngredients)
                {
                    ingredients.Remove(ingredient);
                }
                ingredients.AddRange(previousIngredients);
            }

            if (ingredients != null)
            {
                return ingredients;
            }
            else
            {
                throw new Exception();
            }
        }
       
        // Gets all ingredients used in coffees made by a certain user. Gets all ingredients for every coffee for an
        // admin or owner
        public IEnumerable<IngredientModel> GetAllUsedIngredients(string userId)
        {
            var ingredients = new List<IngredientModel>();
            if (userId==null || _userManager.IsInRole(userId, UserRoles.User))
            {
                ingredients = _db.Ingredients.Where(ing => _db.Coffee.Where(cof => cof.User == null || cof.User.Id == userId)
                                                                         .SelectMany(cof => cof.Ingredients)
                                                                         .Select(ing_in => ing_in.IngredientId)
                                                                         .Contains(ing.IngredientId)).ToList();
            }
            else
            {
                ingredients = _db.Ingredients.Where(ing => _db.Coffee.SelectMany(cof => cof.Ingredients)
                                                            .Select(ing_in => ing_in.IngredientId)
                                                            .Contains(ing.IngredientId)).ToList();
            }

            if (ingredients != null)
            {
                return ingredients;
            }
            else
            {
                throw new Exception();
            }
        }

        // Creates a new ingredient in database and sets default stock quantity for it. Only possible for admin/owner
        public void CreateIngredient(IngredientModel _ingredient)
        {
            _ingredient.IngredientId = Guid.NewGuid();
            _ingredient.QuantityInStock = IngredientStock.DEFAULT_STOCK_QUANTITY;
            _db.Ingredients.Add(_ingredient);
            _db.SaveChanges();
        }

        // Updates an already existing ingredient. Only possible for admin/owner
        public void UpdateIngredient(IngredientModel _ingredient)
        {
            try
            {
                // Update the attributes of the ingredient
                var toChange = FindIngredient(_ingredient.IngredientId);
                toChange.Name = _ingredient.Name;
                toChange.ImgUrl = _ingredient.ImgUrl;
                toChange.Price = _ingredient.Price;
                toChange.Description = _ingredient.Description;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // 
        public void DeleteIngredient(Guid? ingredientId)
        {
            try
            {
                // Also deleting each coffee which has this ingredient in it
                var toDelete = FindIngredient(ingredientId);
                var coffee = GetCoffeeForUserByIngredients(new List<string>() { ingredientId.ToString() }, null);
                foreach (var coffeeItem in coffee)
                {
                    DeleteCoffee(coffeeItem.CoffeeId);
                }
                _db.Ingredients.Remove(toDelete);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Updates an ingredient's quantity in stock 
        public void UpdateIngredientStock(Guid? ingredientId, string quantity)
        {
            try
            {
                var ingredient = FindIngredient(ingredientId);
                ingredient.QuantityInStock += Convert.ToInt32(quantity);
                _db.SaveChanges();

            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Returns a list of quantities of the ingredients 
        public IEnumerable<int> GetSelectedIngredientQuantitiesForCoffee(Guid? id, List<string> selectedIngredientIds)
        {
            // Needed to ensure correct order
            List<int> quantities = new List<int>();
            foreach (string ingredientId in selectedIngredientIds)
            {
                Guid ingId = Guid.Parse(ingredientId);
                // Get the quantities from the ingredient in coffee relations
                var ingredients = _db.IngredientInCoffee.Where(ingc => ingc.CoffeeId == id.Value && ingc.IngredientId == ingId)
                                                     .Select(ingc => ingc.Quantity).ToList();
                if (ingredients != null)
                {
                    quantities.Add(ingredients.First());
                }
                else
                {
                    throw new Exception();
                }
            }
            return quantities;
        }

        // Gets the total ingredient price for the coffee (not including the coffee base price)
        private decimal GetIngredientPrice(CreateCoffeeViewModel coffeeViewModel)
        {
            decimal ingredientPrice = 0;
            for (int i = 0; i < coffeeViewModel.selectedIngredients.Count; i++)
            {
                try
                {
                    var ingredient = FindIngredient(Guid.Parse(coffeeViewModel.selectedIngredients.ElementAt(i)));
                    ingredientPrice += ingredient.Price * GetPriceMultiplierForCoffeeSize(coffeeViewModel.Size)
                                                    * coffeeViewModel.selectedIngredientsQuantity.ElementAt(i);
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
            return ingredientPrice;
        }

        // Returns all database ingredients sorted according to the price
        public IEnumerable<IngredientModel> GetSortedIngredients(bool isAscending)
        {
            try
            {
                var allIngredients = _repository.GetIngredients();
                if (isAscending)
                    allIngredients = allIngredients.OrderBy(x => x.Price).ToList();
                else
                    allIngredients = allIngredients.OrderByDescending(x => x.Price).ToList();

                if (allIngredients != null)
                {
                    return allIngredients;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // Gets the total least used ingredient 
        public IngredientModel GetLeastUsedIngredient()
        {
            var ingredient = _db.Ingredients.OrderBy(ing => ing.TotalQuantityUsed).First();
            if (ingredient != null)
            {
                return ingredient;
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets the total most used ingredient
        public IngredientModel GetMostUsedIngredient()
        {
            var ingredientsOrdered = _db.Ingredients.OrderBy(ing => ing.TotalQuantityUsed).ToList();
            if (ingredientsOrdered != null)
            {
                return ingredientsOrdered.Last();
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets the most used ingredient this week
        public IngredientModel GetMostUsedIngredientWeek()
        {
            var ingredientsOrdered = _db.Ingredients.OrderBy(ing => ing.QuantityUsedLastWeek).ToList();
            if (ingredientsOrdered != null)
            {
                return ingredientsOrdered.Last();
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets the least sold ingredient this week
        public IngredientModel GetLeastUsedIngredientWeek()
        {
            var ingredient = _db.Ingredients.OrderBy(ing => ing.QuantityUsedLastWeek).First();
            if (ingredient != null)
            {
                return ingredient;
            }
            else
            {
                throw new Exception();
            }
        }
        #endregion

        #region OrderItem
        // Finds an order item according to its id
        public OrderItemModel FindOrderitem(Guid? orderItemId)
        {
            var orderItem = _db.OrderItems.Find(orderItemId);
            if (orderItem != null)
            {
                return orderItem;
            }
            else
            {
                throw new Exception();
            }
        }

        // Adds an order item to database
        public void AddOrderItem(OrderItemModel orderItemModel)
        {
            _db.OrderItems.Add(orderItemModel);
            _db.SaveChanges();
            return;
        }
        #endregion

        #region Order
        // Finds an order from database according to provided id
        public OrderModel FindOrder(Guid? orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                return order;
            }
            else
            {
                throw new Exception();
            }
        }

        // Gets all user specific orders. Admin/owner sees all made orders
        public IEnumerable<OrderModel> GetOrdersForUser(string userId)
        {
            var orders = _db.Orders.ToList();

            if (_userManager.IsInRole(userId, UserRoles.User))
                orders = orders.Where(ord => ord.User != null && ord.User.Id == userId).ToList();

            if (orders != null)
            {
                return orders;
            }
            else
            {
                throw new Exception();
            }

        }

        // Creates a new order according to provided order items, the user id and address. Only available to users
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

            foreach (var item in orderItems)
            {
                var coffee = _db.Coffee.Find(item.Coffee.CoffeeId);
                OrderItemModel newOrderItem = new OrderItemModel();
                if (coffee != null)
                {
                    newOrderItem.OrderItemId = Guid.NewGuid();
                    newOrderItem.Quantity = item.Quantity;
                    newOrderItem.Order = newOrder;
                    newOrderItem.Coffee = coffee;
                    _db.OrderItems.Add(newOrderItem);
                    newOrder.OrderItems.Add(newOrderItem);
                }
                else
                {
                    throw new Exception();
                }
            }
            _db.Orders.Add(newOrder);
            _db.SaveChanges();
        }

        // Deletes an order according to its order id. Can be done either by user if the order is inactive or
        // by admin/owner if the order is finished or cancelled
        public void DeleteOrder(Guid? orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                // Also removes every order item in it
                _db.OrderItems.RemoveRange(order.OrderItems);
                _db.Orders.Remove(order);
                _db.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
        }

        // Changes an order status from inactive to pending(active). Activates an order. 
        // Can only be done by the user which created the order which by default is inactive.
        public void ActivateOrder(Guid? orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.PENDING;
                order.OrderTime = DateTime.Now;
                _db.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
        }

        // Deactives an already pending(active) order chaning its status to inactive. Can be done either by user
        // or admin/owner
        public void DeactivateOrder(Guid? orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.INACTIVE;
                order.OrderTime = DateTime.MaxValue;
                _db.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
        }

        // Discards an order which was either finished or cancelled. Can only be done by a user.
        // Discarding doesn't delete the order from database in order to keep the orders history.
        public void DiscardOrder(Guid? orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                order.User = null;
                _db.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
        }

        // Finishes a pending order making it eligible for rating. Can only be done by an admin/owner on a pending
        // valid order.
        public void FinishOrder(Guid? orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.FINISHED;
                order.OrderFinishTime = DateTime.Now;
                foreach (var orderItem in order.OrderItems)
                {
                    // Also updating coffee and ingredient statistics here
                    var coffee = FindCoffee(orderItem.Coffee.CoffeeId);
                    coffee.TotalQuantitySold += orderItem.Quantity;
                    var ingredientsInCoffee = GetIngredientsInCoffee(coffee.CoffeeId);
                    foreach (var ingredientInCoffee in ingredientsInCoffee)
                    {
                        var ing = FindIngredient(ingredientInCoffee.IngredientId);
                        ing.TotalQuantityUsed += ingredientInCoffee.Quantity;
                    }
                }
                _db.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
        }

        // Force cancels a pending order. Can only be done by admin/owner 
        public void ForceCancelOrder(Guid? orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.CANCELLED;
                _db.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
        }

        // Rates a finished order. Can only be done by a user on a custom finished order
        public void RateOrder(Guid? orderId, string grade)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderRating = Convert.ToInt32(grade);
                _db.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
        }


        // Returns a view model regarding information about cancellability of an order
        public IEnumerable<CancelOrderViewModel> GetOrderCancels(IEnumerable<OrderModel> orders)
        {
            List<CancelOrderViewModel> cancels = new List<CancelOrderViewModel>();
            foreach (var order in orders)
            {
                cancels.Add(new CancelOrderViewModel
                {
                    OrderId = order.OrderId,
                    Cancellable = GetCancelOrderOption(order)
                });
            }
            return cancels;
        }

        // Returns whether an order is able to be cancelled by a user or not. An order can be cancelled by a user
        // if either the order is inactve or its not in the first total 5 orders ordered.
        private bool GetCancelOrderOption(OrderModel order)
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

        // Returns all order statuses using reflection
        public IEnumerable<string> GetAllOrderStatuses()
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
        #endregion

        internal void Dispose()
        {
            _db.Dispose();
        }
    }
}
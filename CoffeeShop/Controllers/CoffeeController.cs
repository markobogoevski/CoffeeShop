namespace CoffeeShop.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Web;
    using System.Web.Mvc;
    using CoffeeShop.Enumerations;
    using CoffeeShop.Models;
    using CoffeeShop.Models.ViewModels;
    using CoffeeShop.Services;
    using Microsoft.AspNet.Identity;

    [Authorize]
    public class CoffeeController : Controller
    {
        private Repository _repository;
        public CoffeeController()
        {
            _repository = Repository.GetInstance();
        }

        // Post: ApiCallFilter
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ApiCallFilter()
        {
            if (Request.IsAjaxRequest())
            {
                var ingredients = _repository.GetAllUsedIngredients(User.Identity.GetUserId())
                                             .ToList();
                ViewBag.Ingredients = ingredients;
                List<CoffeeModel> coffee = null;
                List<string> ingredientIds = new List<string>();
                foreach (IngredientModel ingredient in ingredients)
                {
                    if (Request.QueryString[ingredient.Name] != null)
                        ingredientIds.Add(Request.QueryString[ingredient.Name]);
                }
                if (ingredientIds.Count >= 1)
                {
                    coffee = _repository.GetCoffeeForUserByIngredients(ingredientIds, User.Identity.GetUserId())
                                        .ToList();
                }
                else
                {
                    coffee = _repository.GetAllCoffeeForUser(User.Identity.GetUserId())
                                        .ToList();
                }

                return Json(coffee.Select(cof => cof.CoffeeId), JsonRequestBehavior.AllowGet);
            }
            else
                return new HttpNotFoundResult();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                var ingredients = _repository.GetAllUsedIngredients(User.Identity.GetUserId())
                                             .ToList();
                ViewBag.Ingredients = ingredients;
                List<CoffeeModel> coffee = null;
                List<string> ingredientIds = new List<string>();
                coffee = _repository.GetAllCoffeeForUser(User.Identity.GetUserId())
                                        .ToList();
                if (User.IsInRole(UserRoles.Admin) || User.IsInRole(UserRoles.Owner))
                    return View(coffee);
                else
                    return View("IndexUser", coffee);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
           
        }

        // GET: Coffee/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid id, bool statistics=false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                CoffeeModel coffeeModel = _repository.FindCoffee(id);
                var ingredientsForCoffee = _repository.GetIngredientsInCoffee(coffeeModel.CoffeeId)
                                                      .ToList();
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                if (statistics)
                {
                    ViewBag.Statistics = true;
                    ViewBag.TotalProfit = _repository.GetTotalProfitCoffee(coffeeModel);
                    ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeekCoffee(coffeeModel);
                }
                return View(coffeeModel);

            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/StatisticsDetails/5
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult StatisticsDetails(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                return RedirectToAction("Details",new { id,statistics = true});
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/CreateCustom
        [Authorize(Roles = UserRoles.User)]
        public ActionResult CreateCustom()
        {
            try
            {
                var Sizes = _repository.GetAllCoffeeSizes().ToList();
                ViewBag.Sizes = Sizes;
                return View(createCoffeeViewModel(null));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }
        
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public ActionResult CreateCustomCoffee(CreateCoffeeViewModel coffeeViewModel, HttpPostedFileBase file, string description)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Create(coffeeViewModel, file, description, custom: true);
                }
                var Sizes = _repository.GetAllCoffeeSizes().ToList();
                ViewBag.Sizes = Sizes;
                coffeeViewModel.availableIngredients = _repository.GetAvailableIngredients(null).Select(item => new IngredientQuantityViewModel
                {
                    Ingredient = item,
                    QuantityInCoffee = 1
                }).ToList();
                return View("CreateCustom",coffeeViewModel);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        private ActionResult AddShoppingCartCustom(CreateCoffeeViewModel coffeeViewModel, HttpPostedFileBase file)
        {
                if (file != null)
                {
                    string pic = Path.GetFileName(file.FileName);
                    string path = Path.Combine(
                    Server.MapPath("~/Content/Images/Coffee"), pic);
                    file.SaveAs(path);

                    // save the image path path to the database
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                    coffeeViewModel.ImgUrl = "/Content/Images/Coffee/" + pic;
                }
                else
                {
                    coffeeViewModel.ImgUrl = "/Content/Images/Coffee/default_coffee.JPG";
                }

            try
            {
                coffeeViewModel.selectedIngredientsQuantity = coffeeViewModel.selectedIngredientsQuantity.Where(quan => quan != 0)
                                                                                                         .ToList();
                _repository.CreateCoffee(coffeeViewModel, User.Identity.GetUserId());
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }
                
        // GET: Coffee/Create
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult Create()
        {
            try
            {
                var Sizes = _repository.GetAllCoffeeSizes().ToList();
                ViewBag.Sizes = Sizes;
                return View(createCoffeeViewModel(null));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Coffee/Create
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult Create(CreateCoffeeViewModel coffeeViewModel, HttpPostedFileBase file, string description, bool custom = false)
        {
            try
            {
                if (!custom)
                {
                    if (coffeeViewModel.BasePrice < 0)
                    {
                        ModelState.AddModelError("BasePrice", "Base price can't be negative");
                    }
                    if (coffeeViewModel.IncomeCoef < 1)
                    {
                        ModelState.AddModelError("IncomeCoef", "Income coefficient can't be less than 1.");
                    }
                }
                
                if (ModelState.IsValid)
                {
                    coffeeViewModel.Description = description;

                    if (file != null)
                    {
                        string pic = Path.GetFileName(file.FileName);
                        string path = Path.Combine(
                        Server.MapPath("~/Content/Images/Coffee"), pic);
                        file.SaveAs(path);

                        // save the image path path to the database
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                        }

                        coffeeViewModel.ImgUrl = "/Content/Images/Coffee/" + pic;
                    }
                    else
                    {
                        coffeeViewModel.ImgUrl = "/Content/Images/Coffee/default_coffee.JPG";
                    }

                    coffeeViewModel.selectedIngredientsQuantity = coffeeViewModel.selectedIngredientsQuantity.Where(quan => quan != 0)
                                                                                                             .ToList();
                    if (custom)
                    {
                        _repository.CreateCoffee(coffeeViewModel, User.Identity.GetUserId());
                    }
                    else
                    {
                        _repository.CreateCoffee(coffeeViewModel, null);
                    }
                    ViewBag.Title = "Coffee Shop blabla";
                    return RedirectToAction("Index");
                }
                
                var Sizes = _repository.GetAllCoffeeSizes().ToList();
                ViewBag.Sizes = Sizes;
                coffeeViewModel.availableIngredients = _repository.GetAvailableIngredients(null).Select(item => new IngredientQuantityViewModel
                {
                    Ingredient = item,
                    QuantityInCoffee = 1
                }).ToList();
                return View(coffeeViewModel);
            }
            catch (Exception)
            {
                if (custom)
                {
                    throw new Exception();
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        // GET: Coffee/Edit/5
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var Sizes = _repository.GetAllCoffeeSizes().ToList();
                ViewBag.Sizes = Sizes;
                return View(createCoffeeViewModel(id));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Coffee/Edit/5
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult Edit(CreateCoffeeViewModel coffeeViewModel, HttpPostedFileBase file, string description)
        {
            try
            {
                if (coffeeViewModel.BasePrice < 0)
                {
                    ModelState.AddModelError("BasePrice", "Base price can't be negative");
                }
                if (coffeeViewModel.IncomeCoef < 1)
                {
                    ModelState.AddModelError("IncomeCoef", "Income coefficient can't be less than 1.");
                }

                if (ModelState.IsValid)
                {
                    coffeeViewModel.Description = description;
                    if (file != null)
                    {
                        string pic = Path.GetFileName(file.FileName);
                        string path = Path.Combine(
                        Server.MapPath("~/Content/Images/Coffee"), pic);
                        file.SaveAs(path);

                        // save the image path path to the database
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                        }

                        coffeeViewModel.ImgUrl = "/Content/Images/Coffee/" + pic;
                    }

                    coffeeViewModel.selectedIngredientsQuantity = coffeeViewModel.selectedIngredientsQuantity.Where(quan => quan != 0).ToList();
                    _repository.EditCoffee(coffeeViewModel);
                    return RedirectToAction("Index");
                }
                var Sizes = _repository.GetAllCoffeeSizes().ToList();
                ViewBag.Sizes = Sizes;
                return View(createCoffeeViewModel(coffeeViewModel.CoffeeId));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
            
        }

        // POST: Coffee/Delete
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult Delete(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.DeleteCoffee(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Coffee/DeleteCustom
        [Authorize(Roles = UserRoles.User)]
        public ActionResult DeleteCustom(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var coffee = _repository.GetAllCoffeeForUser(User.Identity.GetUserId()).Where(cof=>cof.User!=null);
                if (coffee.Select(cof => cof.CoffeeId).Contains(id))
                {
                    _repository.DeleteCoffee(id);
                    return RedirectToAction("Index");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/CoffeeDay
        [AllowAnonymous]
        public ActionResult CoffeeDay()
        {
            try
            {
                var coffee = _repository.GetAllCoffeeForUser(User.Identity.GetUserId()).ToList();

                if (coffee.Count == 0)
                {
                    ViewBag.Empty = true;
                    return PartialView("_DailyDeal");
                }

                var coffeeDay = _repository.GetCoffeeDay();
                var ingredientsForCoffee = _repository.GetIngredientsInCoffee(coffeeDay.CoffeeId)
                                                      .ToList();
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                return PartialView("_DailyDeal", coffeeDay);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/HideDeal
        [AllowAnonymous]
        public ActionResult HideDeal()
        {
            return PartialView("_HideDeal");
        }

        // GET: Coffee/CoffeeStatistics
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult CoffeeStatistics()
        {
            try
            {
                var coffee = _repository.GetAllCoffeeForUser(User.Identity.GetUserId());
                var coffeeStatistics = _repository.GetCoffeeStatistics(coffee)
                                                  .ToList();
                List<decimal> totalCoffeeProfit = new List<decimal>();
                List<decimal> totalCoffeeProfitWeek = new List<decimal>();

                foreach (var coffeeStatistic in coffeeStatistics) 
                {
                    totalCoffeeProfit.Add(_repository.GetTotalProfitCoffee(coffeeStatistic.Coffee));
                    totalCoffeeProfitWeek.Add(_repository.GetTotalProfitWeekCoffee(coffeeStatistic.Coffee));
                }

                ViewBag.TotalCoffeeProfit = totalCoffeeProfit;
                ViewBag.TotalCoffeeProfitWeek = totalCoffeeProfitWeek;

                return View(coffeeStatistics);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Coffee/UpdateCoffeeQuantity
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult UpdateCoffeeQuantity(string id, string quantity)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.UpdateCoffeeStock(Guid.Parse(id), quantity);
                var coffee = _repository.GetAllCoffeeForUser(User.Identity.GetUserId());
                var coffeeStatistics = _repository.GetCoffeeStatistics(coffee)
                                                  .ToList();

                List<decimal> totalCoffeeProfit = new List<decimal>();
                List<decimal> totalCoffeeProfitWeek = new List<decimal>();

                foreach (var coffeeStatistic in coffeeStatistics)
                {
                    totalCoffeeProfit.Add(_repository.GetTotalProfitCoffee(coffeeStatistic.Coffee));
                    totalCoffeeProfitWeek.Add(_repository.GetTotalProfitWeekCoffee(coffeeStatistic.Coffee));
                }

                ViewBag.TotalCoffeeProfit = totalCoffeeProfit;
                ViewBag.TotalCoffeeProfitWeek = totalCoffeeProfitWeek;

                return View("CoffeeStatistics", coffeeStatistics);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }          
        }

        // GET: Coffee/MostSold
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult MostSold()
        {
            try
            {
                CoffeeModel mostSold = _repository.GetMostSoldCoffee();
                ViewBag.Statistics = true;
                _repository.FixCoffeeQuantity(mostSold.CoffeeId);
                ViewBag.TotalProfit = _repository.GetTotalProfitCoffee(mostSold);
                ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeekCoffee(mostSold);
                var ingredientsForCoffee = _repository.GetIngredientsInCoffee(mostSold.CoffeeId)
                                                      .ToList();
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                return View("Details", mostSold);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/LeastSold 
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult LeastSold()
        {
            try
            {
                CoffeeModel leastSold = _repository.GetLeastSoldCoffee();
                ViewBag.Statistics = true;
                _repository.FixCoffeeQuantity(leastSold.CoffeeId);
                ViewBag.TotalProfit = _repository.GetTotalProfitCoffee(leastSold);
                ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeekCoffee(leastSold);
                var ingredientsForCoffee = _repository.GetIngredientsInCoffee(leastSold.CoffeeId)
                                                      .ToList();
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                return View("Details", leastSold);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/MostSoldWeek 
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult MostSoldWeek()
        {
            try
            {
                CoffeeModel mostSold = _repository.GetMostSoldCoffeeWeek();
                ViewBag.Statistics = true;
                _repository.FixCoffeeQuantity(mostSold.CoffeeId);
                ViewBag.TotalProfit = _repository.GetTotalProfitCoffee(mostSold);
                ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeekCoffee(mostSold);
                var ingredientsForCoffee = _repository.GetIngredientsInCoffee(mostSold.CoffeeId)
                                                      .ToList();
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                return View("Details", mostSold);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/LeastSoldWeek 
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult LeastSoldWeek()
        {
            try
            {
                CoffeeModel leastSold = _repository.GetLeastSoldCoffeeWeek();
                ViewBag.Statistics = true;
                _repository.FixCoffeeQuantity(leastSold.CoffeeId);
                ViewBag.TotalProfit = _repository.GetTotalProfitCoffee(leastSold);
                ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeekCoffee(leastSold);
                var ingredientsForCoffee = _repository.GetIngredientsInCoffee(leastSold.CoffeeId)
                                                      .ToList();
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                return View("Details", leastSold);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        private CreateCoffeeViewModel createCoffeeViewModel(Guid? id)
        {
            var coffeeViewModel = new CreateCoffeeViewModel();
            try
            {
                coffeeViewModel.availableIngredients = _repository.GetAvailableIngredients(id).Select(item => new IngredientQuantityViewModel
                {
                    Ingredient = item,
                    QuantityInCoffee = 1
                }).ToList();
                
                if (id != null)
                {
                    var coffeeToEdit = _repository.FindCoffee(id);
                    coffeeViewModel.CoffeeId = id;
                    coffeeViewModel.selectedIngredients = _repository.GetIngredientsForCoffee(id)
                                                                     .Select(x => x.IngredientId.ToString())
                                                                     .ToList();
                    coffeeViewModel.Name = coffeeToEdit.Name;
                    coffeeViewModel.selectedIngredientsQuantity = _repository.GetSelectedIngredientQuantitiesForCoffee(id, coffeeViewModel.selectedIngredients)
                                                                             .ToList();
                    coffeeViewModel.Size = coffeeToEdit.Size;
                    coffeeViewModel.IncomeCoef = coffeeToEdit.IncomeCoef;
                    coffeeViewModel.ImgUrl = coffeeToEdit.ImgUrl;
                    coffeeViewModel.BasePrice = coffeeToEdit.BasePrice;
                    coffeeViewModel.Description = coffeeToEdit.Description;
                    coffeeViewModel.QuantityInStock = coffeeToEdit.QuantityInStock;
                }
                return coffeeViewModel;
            }
            catch (Exception)
                {
                    throw new Exception();
                }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

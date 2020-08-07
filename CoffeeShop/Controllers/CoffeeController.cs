using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.Models.ViewModels;
using CoffeeShop.Services;

namespace CoffeeShop.Controllers
{
    public class CoffeeController : Controller
    {
        private Repository _repository;
        public CoffeeController()
        {
            _repository = Repository.GetInstance();
        }

        // GET: Coffee
        public ActionResult Index()
        {
            ViewBag.Title = "Coffee shop blabla";
            List<IngredientModel> ingredients = _repository.GetAllUsedIngredients();
            ViewBag.Ingredients = ingredients;
            List<CoffeeModel> coffee = new List<CoffeeModel>();
            List<string> ingredientIds = new List<string>();
            foreach(IngredientModel ingredient in ingredients)
            {
                if (Request.QueryString[ingredient.Name] != null)
                    ingredientIds.Add(Request.QueryString[ingredient.Name]);
            }
            if (ingredientIds.Count >= 1)
            {
                coffee = _repository.GetCoffeeByIngredients(ingredientIds);
            }
            else
            {
                coffee = _repository.GetAllCoffee();
            }
            return View(coffee);
        }

        // GET: Coffee/Details/5
        public ActionResult Details(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                CoffeeModel coffeeModel = _repository.FindCoffee(id);
                List<IngredientInCoffeeModel> ingredientsForCoffee = _repository.GetIngredientsInCoffee(coffeeModel.CoffeeId);
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                return View(coffeeModel);

            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/CreateCustom
        public ActionResult CreateCustom()
        {
            try
            {
                List<string> Sizes = new List<string>();
                Sizes = _repository.GetAllCoffeeSizes();
                ViewBag.Sizes = Sizes;
                List<string> CoffeeTypes = new List<string>();
                CoffeeTypes = _repository.GetAllCoffeeTypes();
                ViewBag.CoffeeTypes = CoffeeTypes;
                return View(createCoffeeViewModel(null));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Coffee/Create
        public ActionResult Create()
        {
            try
            {
                List<string> Sizes = new List<string>();
                Sizes = _repository.GetAllCoffeeSizes();
                ViewBag.Sizes = Sizes;
                return View(createCoffeeViewModel(null));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        private CreateCoffeeViewModel createCoffeeViewModel(Guid? id)
        {
            var coffeeViewModel = new CreateCoffeeViewModel();

            if (id != null)
            {
                try
                {
                    var coffeeToEdit = _repository.FindCoffee(id);
                    coffeeViewModel.CoffeeId = id;
                    coffeeViewModel.selectedIngredients = _repository.GetIngredientsForCoffee(id)
                                                                     .Select(x => x.IngredientId.ToString())
                                                                     .ToList();
                    coffeeViewModel.Name = coffeeToEdit.Name;
                    coffeeViewModel.selectedIngredientsQuantity = _repository.GetSelectedIngredientQuantitiesForCoffee(id,coffeeViewModel.selectedIngredients);
                    coffeeViewModel.Size = coffeeToEdit.Size;
                    coffeeViewModel.IncomeCoef = coffeeToEdit.IncomeCoef;
                    coffeeViewModel.ImgUrl = coffeeToEdit.ImgUrl;
                    coffeeViewModel.BasePrice = coffeeToEdit.BasePrice;
                    coffeeViewModel.Description = coffeeToEdit.Description;
                    coffeeViewModel.QuantityInStock = coffeeToEdit.QuantityInStock;
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
            coffeeViewModel.availableIngredients = _repository.GetAvailableIngredients()
                                                              .ToList();
            return coffeeViewModel;
        }

        // POST: Coffee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateCoffeeViewModel coffeeViewModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string pic = Path.GetFileName(file.FileName);
                    string path = Path.Combine(
                    Server.MapPath("~/Content/Images"), pic);
                    file.SaveAs(path);

                    // save the image path path to the database
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                    coffeeViewModel.ImgUrl = "/Content/Images/" + pic;
                }
                else
                {
                    coffeeViewModel.ImgUrl = "/Content/Images/default_coffee.JPG";
                }

                coffeeViewModel.selectedIngredientsQuantity = coffeeViewModel.selectedIngredientsQuantity.Where(quan => quan != 0).ToList();
                _repository.CreateCoffee(coffeeViewModel);
                ViewBag.Title = "Coffee Shop blabla";
                return RedirectToAction("Index");
            }
            List<string> Sizes = new List<string>();
            Sizes = _repository.GetAllCoffeeSizes();
            ViewBag.Sizes = Sizes;
            return View(coffeeViewModel);
        }

        // GET: Coffee/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                List<string> Sizes = new List<string>();
                Sizes = _repository.GetAllCoffeeSizes();
                ViewBag.Sizes = Sizes;
                return View(createCoffeeViewModel(id));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Coffee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateCoffeeViewModel coffeeViewModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string pic = Path.GetFileName(file.FileName);
                    string path = Path.Combine(
                    Server.MapPath("~/Content/Images"), pic);
                    file.SaveAs(path);

                    // save the image path path to the database
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                    coffeeViewModel.ImgUrl = "/Content/Images/" + pic;
                }
                else
                {
                    coffeeViewModel.ImgUrl = "/Content/Images/default_coffee.JPG";
                }
                _repository.EditCoffee(coffeeViewModel);
                return RedirectToAction("Index");
            }
            List<string> Sizes = new List<string>();
            Sizes = _repository.GetAllCoffeeSizes();
            ViewBag.Sizes = Sizes;
            return View(createCoffeeViewModel(coffeeViewModel.CoffeeId));
        }

        // Post: Coffee/Delete
        public ActionResult Delete(Guid id)
        {
            if (id == null)
            {
                return HttpNotFound();
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

        public ActionResult CoffeeDay()
        {
            var coffeeDay = GetCoffeeDay();
            List<IngredientInCoffeeModel> ingredientsForCoffee = _repository.GetIngredientsInCoffee(coffeeDay.CoffeeId);
            ViewBag.IngredientsForCoffee = ingredientsForCoffee;
            return PartialView("_DailyDeal", coffeeDay);
        }

        public ActionResult HideDeal()
        {
            return PartialView("_HideDeal");
        }

        public ActionResult CoffeeStatistics()
        {
            var coffee = _repository.GetAllCoffee();
            var coffeeStatistics = _repository.GetCoffeeStatistics(coffee);
            return View(coffeeStatistics);
        }

        // Post: Coffee/UpdateCoffeeQuantity
        public ActionResult UpdateCoffeeQuantity(string id,string quantity)
        {
            _repository.UpdateCoffeeStock(id, quantity);
            var coffee = _repository.GetAllCoffee();
            var coffeeStatistics = _repository.GetCoffeeStatistics(coffee);
            return View("CoffeeStatistics",coffeeStatistics);
        }

        public ActionResult MostSold()
        {
            CoffeeModel mostSold = _repository.GetMostSoldCoffee();
            ViewBag.Statistics = true;
            ViewBag.TotalProfit = _repository.GetTotalProfit(mostSold);
            ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeek(mostSold);
            return View("Details",mostSold);
        }

        public ActionResult LeastSold()
        {
            CoffeeModel leastSold = _repository.GetLeastSoldCoffee();
            ViewBag.Statistics = true;
            ViewBag.TotalProfit = _repository.GetTotalProfit(leastSold);
            ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeek(leastSold);
            return View("Details", leastSold);
        }

        public ActionResult MostSoldWeek()
        {
            CoffeeModel mostSold = _repository.GetMostSoldCoffeeWeek();
            ViewBag.Statistics = true;
            ViewBag.TotalProfit = _repository.GetTotalProfit(mostSold);
            ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeek(mostSold);
            return View("Details", mostSold);
        }

        public ActionResult LeastSoldWeek()
        {
            CoffeeModel leastSold = _repository.GetLeastSoldCoffeeWeek();
            ViewBag.Statistics = true;
            ViewBag.TotalProfit = _repository.GetTotalProfit(leastSold);
            ViewBag.TotalProfitWeek = _repository.GetTotalProfitWeek(leastSold);
            return View("Details", leastSold);
        }

        private CoffeeModel GetCoffeeDay()
        {
            CoffeeModel randomCoffee = _repository.GetRandomCoffee();
            randomCoffee.TotalPrice *= 0.7m;
            return randomCoffee;
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

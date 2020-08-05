﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Enumerations;
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
            var coffee = _repository.GetAllCoffee();
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
                ViewBag.Ingredients = _repository.GetIngredients().ToList();
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
                    coffeeViewModel.Size = coffeeToEdit.Size;
                    coffeeViewModel.ImgUrl = coffeeToEdit.ImgUrl;
                    coffeeViewModel.BasePrice = coffeeToEdit.BasePrice;
                    coffeeViewModel.Description = coffeeToEdit.Description;
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
            coffeeViewModel.availableIngredients = _repository.GetIngredients()
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
        public ActionResult Edit(CreateCoffeeViewModel coffeeViewModel)
        {
            if (ModelState.IsValid)
            {
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
            return PartialView("_DailyDeal", coffeeDay);
        }

        public ActionResult HideDeal()
        {
            return PartialView("_HideDeal");
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

using System;
using System.Net;
using System.Web.Mvc;
using CoffeeShop.Models;
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

        // View all premade available coffee
        // GET: CoffeeComponentModels
        public ActionResult Index()
        {
            ViewBag.Title = "Coffee Shop blabla";
            var coffee = _repository.GetAllCoffee();
            return View(coffee);
        }

        // View information about a specific coffee from the premade available ones
        // GET: CoffeeComponentModels/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                return View(_repository.GetCoffee(id));
            }
            catch(Exception)
            {
                return HttpNotFound();
            }
        }

        // Create your own custom coffee
        // GET: CoffeeComponentModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CoffeeComponentModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Size,Description,Name,ImgUrl")] CoffeeComponentModel coffeeComponentModel)
        {
            if (ModelState.IsValid)
            {
                // Need to use view model here and a factory to create the wanted coffee with the
                // decorator pattern
                coffeeComponentModel.Id = Guid.NewGuid();
                // Add to shopping cart here
                return RedirectToAction("Index");
            }

            return View(coffeeComponentModel);
        }

        // Edit some of the premade default coffee
        // GET: CoffeeComponentModels/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                return View(_repository.GetCoffee(id));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: CoffeeComponentModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Size,Description,Name,ImgUrl")] CoffeeComponentModel coffeeComponentModel)
        {
            if (ModelState.IsValid)
            {
                _repository.EditCoffee(coffeeComponentModel);
                return RedirectToAction("Index");
            }
            return View(coffeeComponentModel);
        }

        // GET: CoffeeComponentModels/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                return View(_repository.GetCoffee(id));
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: CoffeeComponentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                CoffeeComponentModel coffeeComponentModel = _repository.GetCoffee(id);
                _repository.RemoveCoffee(coffeeComponentModel);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
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

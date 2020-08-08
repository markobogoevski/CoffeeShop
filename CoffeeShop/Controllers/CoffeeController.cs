using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.Services;

namespace CoffeeShop.Controllers
{
    public class CoffeeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private Repository _repository;
        public CoffeeController()
        {
            _repository = Repository.GetInstance();
        }

        // GET: CoffeeModels
        public ActionResult Index()
        {
            var coffee = _repository.GetAllCoffee();
            return View(coffee);
        }

        // GET: CoffeeModels/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoffeeModel coffeeModel = db.Coffee.Find(id);
            if (coffeeModel == null)
            {
                return HttpNotFound();
            }
            return View(coffeeModel);
        }

        // GET: CoffeeModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CoffeeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CoffeeId,Name,Price,Size,ImgUrl,Description")] CoffeeModel coffeeModel)
        {
            if (ModelState.IsValid)
            {
                coffeeModel.CoffeeId = Guid.NewGuid();
                db.Coffee.Add(coffeeModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(coffeeModel);
        }

        // GET: CoffeeModels/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoffeeModel coffeeModel = db.Coffee.Find(id);
            if (coffeeModel == null)
            {
                return HttpNotFound();
            }
            return View(coffeeModel);
        }

        // POST: CoffeeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CoffeeId,Name,Price,Size,ImgUrl,Description")] CoffeeModel coffeeModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(coffeeModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(coffeeModel);
        }

        // GET: CoffeeModels/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoffeeModel coffeeModel = db.Coffee.Find(id);
            if (coffeeModel == null)
            {
                return HttpNotFound();
            }
            return View(coffeeModel);
        }

        // POST: CoffeeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CoffeeModel coffeeModel = db.Coffee.Find(id);
            db.Coffee.Remove(coffeeModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

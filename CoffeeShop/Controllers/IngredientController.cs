using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;

namespace CoffeeShop.Controllers
{
    public class IngredientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ingredient
        public ActionResult Index()
        {
            return View(db.Ingredients.ToList());
        }

        // GET: Ingredient/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientModel ingredientModel = db.Ingredients.Find(id);
            if (ingredientModel == null)
            {
                return HttpNotFound();
            }
            return View(ingredientModel);
        }

        // GET: Ingredient/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ingredient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IngredientId,Name,Price,ImgUrl,Description")] IngredientModel ingredientModel)
        {
            if (ModelState.IsValid)
            {
                ingredientModel.IngredientId = Guid.NewGuid();
                db.Ingredients.Add(ingredientModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ingredientModel);
        }

        // GET: Ingredient/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientModel ingredientModel = db.Ingredients.Find(id);
            if (ingredientModel == null)
            {
                return HttpNotFound();
            }
            return View(ingredientModel);
        }

        // POST: Ingredient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IngredientId,Name,Price,ImgUrl,Description")] IngredientModel ingredientModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingredientModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ingredientModel);
        }

        // GET: Ingredient/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientModel ingredientModel = db.Ingredients.Find(id);
            if (ingredientModel == null)
            {
                return HttpNotFound();
            }
            return View(ingredientModel);
        }

        // POST: Ingredient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            IngredientModel ingredientModel = db.Ingredients.Find(id);
            db.Ingredients.Remove(ingredientModel);
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

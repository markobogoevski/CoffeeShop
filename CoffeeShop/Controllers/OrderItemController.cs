using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.Models.Order;
using CoffeeShop.Services;

namespace CoffeeShop.Controllers
{
    public class OrderItemController : Controller
    {
        private Repository _repository;
        public OrderItemController()
        {
            _repository = Repository.GetInstance();
        }

        private ApplicationDbContext db { get; set; }

        // GET: OrderItem
        public ActionResult Index()
        {
            return View(db.OrderItems.ToList());
        }

        // GET: OrderItem/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItemModel orderItemModel = db.OrderItems.Find(id);
            if (orderItemModel == null)
            {
                return HttpNotFound();
            }
            return View(orderItemModel);
        }

        // GET: OrderItem/Create
        public ActionResult Create(string id)
        {
            var coffee = _repository.GetCoffee(id);
            OrderItemModel newOrderItemModel = new OrderItemModel()
            {
                Coffee = coffee
            };
            List<IngredientInCoffeeModel> ingredientsForCoffee = _repository.GetIngredientsInCoffee(coffee.CoffeeId);
            ViewBag.IngredientsForCoffee = ingredientsForCoffee;
            return View(newOrderItemModel);
        }

        [HttpPost]
        public ActionResult Create(string id, string quantity)
        {
            return RedirectToAction("AddToCart", "Cart", new { coffeeId = id, quantity = quantity});
        }

        // GET: OrderItem/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItemModel orderItemModel = db.OrderItems.Find(id);
            if (orderItemModel == null)
            {
                return HttpNotFound();
            }
            return View(orderItemModel);
        }

        // POST: OrderItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderItemId,Quantity")] OrderItemModel orderItemModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderItemModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderItemModel);
        }

        // GET: OrderItem/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderItemModel orderItemModel = db.OrderItems.Find(id);
            if (orderItemModel == null)
            {
                return HttpNotFound();
            }
            return View(orderItemModel);
        }

        // POST: OrderItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            OrderItemModel orderItemModel = db.OrderItems.Find(id);
            db.OrderItems.Remove(orderItemModel);
            db.SaveChanges();
            return RedirectToAction("Index");
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

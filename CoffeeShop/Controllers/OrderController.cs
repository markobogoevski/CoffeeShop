using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CoffeeShop.Enumerations;
using CoffeeShop.Models;
using CoffeeShop.Models.Order;
using CoffeeShop.Services;
using Microsoft.AspNet.Identity;

namespace CoffeeShop.Controllers
{
    public class OrderController : Controller
    {
        private Repository _repository;

        public OrderController()
        {
            _repository = Repository.GetInstance();
        }

        // Get all orders
        // GET: Order
        public ActionResult Index()
        {
            if (User.IsInRole(UserRoles.Owner) || User.IsInRole(UserRoles.Admin))
            {
                return View(_repository.GetAllOrders());
            }
            else
                return View(_repository.GetAllOrdersForUser(User.Identity.GetUserId()));
        }

        // Get information about a specific order
        // GET: OrderModels/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderModel orderModel = db.Orders.Find(id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // GET: OrderModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,Address,OrderStatus,OrderTime,OrderRating")] OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {
                orderModel.OrderId = Guid.NewGuid();
                db.Orders.Add(orderModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orderModel);
        }

        // GET: OrderModels/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderModel orderModel = db.Orders.Find(id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: OrderModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,Address,OrderStatus,OrderTime,OrderRating")] OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderModel);
        }

        // GET: OrderModels/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderModel orderModel = db.Orders.Find(id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: OrderModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            OrderModel orderModel = db.Orders.Find(id);
            db.Orders.Remove(orderModel);
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

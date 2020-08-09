using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.Models.Order;
using CoffeeShop.Services;
using Microsoft.AspNet.Identity;

namespace CoffeeShop.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Repository _repository;

        public OrderController()
        {
            _repository = Repository.GetInstance();
        }

        // GET: Order
        public ActionResult Index()
        {
            var orders = _repository.GetOrders(User.Identity.GetUserId());
            if (orders.Count == 0)
                ViewBag.Empty = true;
            ViewBag.OrderStatus = _repository.GetAllOrderStatuses();
            var cancellables = _repository.GetOrderCancels(orders, User.Identity.GetUserId());
            ViewBag.Cancellable = cancellables;
            return View(orders);
        }

        // GET: Order/Details/5
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

        // GET: Order/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(List<OrderItemModel> OrderItems)
        {
            string Address = Request.QueryString["Address"];
            if ((OrderItems != null && OrderItems.Count <= 0) || (OrderItems == null))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "There was a problem with placing your order. Please try again later" });
            }
            else
            {
                _repository.CreateOrder(OrderItems, User.Identity.GetUserId(), Address);
                Response.StatusCode = (int)HttpStatusCode.OK;
                Session["cart"] = null;
                return Json(new { message = "Your order has been placed" });
            }
        }

        // GET: Order/Edit/5
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

        // POST: Order/Edit/5
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

        // GET: Order/Cancel/5
        public ActionResult Cancel(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.DeactivateOrder(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Order/Rate
        public ActionResult RateOrder(string id, string grade)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.RateOrder(id, grade);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Order/Finish/5
        public ActionResult Finish(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.FinishOrder(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Order/ForceCancel/5
        public ActionResult ForceCancel(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.ForceCancelOrder(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Order/Discard/5
        public ActionResult Discard(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.DiscardOrder(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Order/Activate/5
        public ActionResult Activate(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.ActivateOrder(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Order/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.DeleteOrder(id);
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

namespace CoffeeShop.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using CoffeeShop.Enumerations;
    using CoffeeShop.Models.Order;
    using CoffeeShop.Services;
    using Microsoft.AspNet.Identity;

    [Authorize]
    public class OrderController : Controller
    {
        private Repository _repository;

        public OrderController()
        {
            _repository = Repository.GetInstance();
        }

        // GET: Order
        public ActionResult Index()
        {
            try
            {
                var orders = _repository.GetOrdersForUser(User.Identity.GetUserId())
                                        .ToList();
                if (orders.Count() == 0)
                    ViewBag.Empty = true;
                ViewBag.OrderStatus = _repository.GetAllOrderStatuses().ToList();
                var cancellables = _repository.GetOrderCancels(orders).ToList();
                ViewBag.Cancellable = cancellables;
                return View(orders);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
         
        }

        // GET: Order/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var order = _repository.FindOrder(id);
                return View(order);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
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
                try
                {
                    _repository.CreateOrder(OrderItems, User.Identity.GetUserId(), Address);
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    Session["cart"] = null;
                    return Json(new { message = "Your order has been placed" });
                }
                catch (Exception)
                {
                    return HttpNotFound();
                }
            }
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
        [Authorize(Roles = UserRoles.User)]
        public ActionResult RateOrder(string id, string grade)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                _repository.RateOrder(Guid.Parse(id), grade);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Order/Finish/5
        [Authorize(Roles = UserRoles.Owner + "," + UserRoles.Admin)]
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
        [Authorize(Roles = UserRoles.Owner + "," + UserRoles.Admin)]
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
        [Authorize(Roles = UserRoles.User)]
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
        [Authorize(Roles = UserRoles.User)]
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
                _repository.DeleteOrder(id,User.Identity.GetUserId());
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

namespace CoffeeShop.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Net;
    using System.Web.Mvc;
    using CoffeeShop.Enumerations;
    using CoffeeShop.Models;
    using CoffeeShop.Models.Order;
    using CoffeeShop.Services;

    [Authorize]
    public class OrderItemController : Controller
    {
        private Repository _repository;
        public OrderItemController()
        {
            _repository = Repository.GetInstance();
        }

        // GET: OrderItem/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                OrderItemModel orderItemModel = _repository.FindOrderitem(id);
                return View(orderItemModel);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: OrderItem/Create
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult Create(string id)
        {
            try
            {
                var coffee = _repository.FindCoffee(Guid.Parse(id));
                OrderItemModel newOrderItemModel = new OrderItemModel()
                {
                    Coffee = coffee
                };
                var ingredientsForCoffee = _repository.GetIngredientsInCoffee(coffee.CoffeeId);
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                return View(newOrderItemModel);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult Create(string id, string quantity)
        {
            return RedirectToAction("AddToCart", "Cart", new { coffeeId = id, quantity });
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

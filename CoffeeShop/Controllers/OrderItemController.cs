namespace CoffeeShop.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using CoffeeShop.Enumerations;
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
        [Authorize(Roles = UserRoles.User)]
        public ActionResult Create(string id, bool? daily)
        {
            try
            {
                var coffee = _repository.FindCoffee(Guid.Parse(id));
                if (daily.HasValue && daily.Value==true)
                    coffee.TotalPrice *= 0.7m;

                OrderItemModel newOrderItemModel = new OrderItemModel()
                {
                    Coffee = coffee
                };
                var ingredientsForCoffee = _repository.GetIngredientsInCoffee(coffee.CoffeeId)
                                                      .ToList();
                ViewBag.IngredientsForCoffee = ingredientsForCoffee;
                if (daily.HasValue)
                {
                    ViewBag.Daily = "true";
                }
                else
                {
                    ViewBag.Daily = "false";
                }

                return View(newOrderItemModel);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public ActionResult Create(string id, string quantity, string daily)
        {
            return RedirectToAction("AddToCart", "Cart", new { coffeeId = id, quantity, daily });
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

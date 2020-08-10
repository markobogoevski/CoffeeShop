namespace CoffeeShop.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using CoffeeShop.Models;
    using CoffeeShop.Models.Order;
    using CoffeeShop.Services;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    [Authorize]
    public class CartController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private Repository _repository;

        public CartController()
        {
            var userStore = new UserStore<ApplicationUser>();
            _userManager = new UserManager<ApplicationUser>(userStore);
            _repository = Repository.GetInstance();
        }

        // View all order items in a cart for a user
        // GET: Cart
        public ActionResult Index()
        {
            object sessionCart = Session["cart"];
            if(sessionCart == null)
            {
                ViewBag.Valid = "False";
            }
            else
            {
                ViewBag.Valid = "True";
            }
            return View(sessionCart);
        }

        // POST: Cart/AddToCart
        public ActionResult AddToCart(string coffeeId, string quantity)
        {
            try
            {
                Guid coffeeGuid = Guid.Parse(coffeeId);
                var coffee = _repository.FindCoffee(coffeeGuid);
                object sessionCart = Session["cart"];
                int quantityNumber = Convert.ToInt32(quantity);

                if (sessionCart != null)
                {
                    var cart = (OrderModel)sessionCart;
                    if (cart.OrderItems.Any(item => item.Coffee.CoffeeId == coffeeGuid))
                    {
                        cart.OrderItems.Find(item => item.Coffee.CoffeeId == coffeeGuid).Quantity += quantityNumber;
                    }
                    else
                    {
                        cart.OrderItems.Add(new OrderItemModel
                        {
                            Quantity = quantityNumber,
                            Coffee = coffee
                        });
                    }
                    Session["cart"] = cart;
                }
                else
                {
                    var cart = new OrderModel();
                    cart.OrderItems.Add(new OrderItemModel
                    {
                        Quantity = quantityNumber,
                        Coffee = coffee
                    });
                    Session["cart"] = cart;
                }
                return View("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Cart/RemoveCoffeeFromOrder/id/quantity
        public ActionResult RemoveCoffeeFromOrder(string id, string quantity)
        {
            OrderModel cart = (OrderModel)Session["cart"];
            if (cart == null)
            {
                return HttpNotFound();
            }

            var coffeeToRemove = cart.OrderItems.First(item => item.Coffee.CoffeeId == Guid.Parse(id));

            if (coffeeToRemove == null)
                return HttpNotFound();

            coffeeToRemove.Quantity -= Convert.ToInt32(quantity);
            if (coffeeToRemove.Quantity == 0)
            {
                cart.OrderItems.Remove(coffeeToRemove);
            }

            Session["cart"] = cart;
            if (cart.OrderItems.Count == 0)
            {
                Session["cart"] = null;
            }

            return RedirectToAction("Index", "Cart");
        }

        // POST: Cart/RemoveOrderItemFromCart/id
        public ActionResult RemoveOrderItemFromCart(string id)
        {
            OrderModel cart = (OrderModel)Session["cart"];
            if (cart == null)
            {
                return HttpNotFound();
            }

            var orderToRemove = cart.OrderItems.Find(item => item.OrderItemId == Guid.Parse(id));

            if (orderToRemove == null)
                return HttpNotFound();

            cart.OrderItems.Remove(orderToRemove);
            Session["cart"] = cart;
            if (cart.OrderItems.Count == 0)
            {
                Session["cart"] = null;
            }

            return RedirectToAction("Index", "Cart");
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

using System;
using System.Linq;
using System.Web.Mvc;
using CoffeeShop.Enumerations;
using CoffeeShop.Models;
using CoffeeShop.Models.Order;
using CoffeeShop.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CoffeeShop.Controllers
{
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
        public ActionResult AddToCart(string orderItemId)
        {
            var orderItemToAdd = _repository.GetOrderItem(Guid.Parse(orderItemId));
            var coffeeId = orderItemToAdd.Coffee.CoffeeId;
            object sessionCart = Session["cart"];

            if (sessionCart != null)
            {
                var cart = (OrderModel)sessionCart;
                if (cart.OrderItems.Any(item => item.Coffee.CoffeeId == coffeeId))
                {
                    cart.OrderItems.Find(item => item.Coffee.CoffeeId == coffeeId).Quantity += orderItemToAdd.Quantity;
                }
                else
                {
                    cart.OrderItems.Add(orderItemToAdd);
                }
                Session["cart"] = cart;
            }
            else
            {
                var cart = new OrderModel();
                cart.OrderItems.Add(orderItemToAdd);
                Session["cart"] = cart;
            }
            return View("Index");
        }

        public ActionResult RemoveCoffeeFromOrder(string id, string quantity)
        {
            OrderModel cart = (OrderModel)Session["cart"];
            if (cart == null)
            {
                return HttpNotFound();
            }

            var coffeeToRemove = cart.OrderItems.First(item => item.Coffee.CoffeeId == Guid.Parse(id));
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

        public ActionResult RemoveOrderItemFromCart(string id)
        {
            OrderModel cart = (OrderModel)Session["cart"];
            if (cart == null)
            {
                return HttpNotFound();
            }

            var orderToRemove = cart.OrderItems.Find(item => item.OrderItemId == Guid.Parse(id));
            cart.OrderItems.Remove(orderToRemove);
            Session["cart"] = cart;
            if (cart.OrderItems.Count == 0)
            {
                Session["cart"] = null;
            }

            return RedirectToAction("Index", "Cart");
        }

        public ActionResult CreateOrderFromCart(string address)
        {
            OrderModel cart = (OrderModel)Session["cart"];
            if (cart == null)
            {
                return HttpNotFound();
            }

            cart.OrderId = Guid.NewGuid();
            cart.Address = address;
            cart.OrderStatus = OrderStatus.INACTIVE;
            cart.User = _userManager.FindById(User.Identity.GetUserId());

            foreach (OrderItemModel orderItemModel in cart.OrderItems)
            {
                orderItemModel.OrderItemId = cart.OrderId;
                orderItemModel.Order = cart;
            }

            return View();
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

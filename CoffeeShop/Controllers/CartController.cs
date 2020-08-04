using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CoffeeShop.Enumerations;
using CoffeeShop.Models;
using CoffeeShop.Models.Order;
using CoffeeShop.Services;

namespace CoffeeShop.Controllers
{
    public class CartController : Controller
    {
        private Repository _repository;

        public CartController()
        {
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
        public ActionResult AddToCart(Guid coffeeId, int quantity)
        {
            try
            {
                CoffeeComponentModel coffeeToAdd = _repository.GetCoffee(coffeeId);
                object sessionCart = Session["cart"];
                if (sessionCart != null)
                {
                    var cart = (OrderModel)sessionCart;
                    if (cart.OrderItems.Any(item => item.Coffee.Id == coffeeId))
                    {
                        cart.OrderItems.Find(item => item.Coffee.Id == coffeeId).Quantity += quantity;
                    }
                    else
                    {
                        cart.OrderItems.Add(new OrderItemModel
                        {
                            Coffee = coffeeToAdd,
                            Quantity=1,
                            OrderItemId = Guid.NewGuid()
                        });
                    }
                    Session["cart"] = cart;
                }
                else
                {
                    var cart = new OrderModel();
                    cart.OrderItems.Add(new OrderItemModel
                    {
                        Coffee = coffeeToAdd,
                        Quantity = 1,
                        OrderItemId = Guid.NewGuid()
                    });
                    Session["cart"] = cart;
                }
                return View();
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        public ActionResult RemoveCoffeeFromOrder(Guid coffeeId)
        {
            OrderModel cart = (OrderModel)Session["cart"];
            if (cart == null)
            {
                return HttpNotFound();
            }

            var coffeeToRemove = cart.OrderItems.First(item => item.Coffee.Id == coffeeId);
            coffeeToRemove.Quantity--;
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

        public ActionResult RemoveOrderItemFromCart(Guid orderId)
        {
            OrderModel cart = (OrderModel)Session["cart"];
            if (cart == null)
            {
                return HttpNotFound();
            }

            var orderToRemove = cart.OrderItems.Find(item => item.OrderItemId == orderId);
            cart.OrderItems.Remove(orderToRemove);
            Session["cart"] = cart;
            if (cart.OrderItems.Count == 0)
            {
                Session["cart"] = null;
            }

            return RedirectToAction("Index", "Cart");
        }

        public ActionResult EnterAddress()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

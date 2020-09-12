namespace CoffeeShop.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using CoffeeShop.Enumerations;
    using CoffeeShop.Models.Order;
    using CoffeeShop.Services;
    using Microsoft.AspNet.Identity;

    [Authorize(Roles=UserRoles.User)]
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
            try
            {
                OrderModel sessionCart = (OrderModel)Session["cart"];
                if (sessionCart == null)
                {
                    ViewBag.Valid = "False";
                    return View(sessionCart);
                }

                var allCoffeeIds = _repository.GetAllCoffeeForUser(User.Identity.GetUserId())
                                            .Select(cof => cof.CoffeeId)
                                            .ToList();

                // Filtering out order items which have deleted coffees in them
                var toRemove = sessionCart.OrderItems.Where(ordI => !(allCoffeeIds.Contains(ordI.Coffee.CoffeeId)))
                                                        .Select(ordI => ordI.OrderItemId)
                                                        .ToList();
                foreach (var id in toRemove)
                {
                    RemoveOrderItemFromCart(id.ToString());
                }

                OrderModel cart = (OrderModel)Session["cart"];
                if (cart == null)
                {
                    ViewBag.Valid = "False";
                }
                else
                {
                    ViewBag.Valid = "True";
                }
                return View(cart);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Cart/AddToCart
        public ActionResult AddToCart(string coffeeId, string quantity, string daily, string size, string price)
        {
            try
            {
                bool dailyCoffee = Boolean.Parse(daily);
                Guid coffeeGuid = Guid.Parse(coffeeId);
                var coffee = _repository.FindCoffee(coffeeGuid);
                object sessionCart = Session["cart"];
                int quantityNumber = Convert.ToInt32(quantity);
                decimal priceD = Convert.ToDecimal(price);

                coffee.TotalPrice = priceD;
                if (sessionCart != null)
                {
                    var cart = (OrderModel)sessionCart;
                    if (cart.OrderItems.Any(item => item.Coffee.CoffeeId == coffeeGuid && item.CoffeeSize == size))
                    {
                        cart.OrderItems.Find(item => item.Coffee.CoffeeId == coffeeGuid).Quantity += quantityNumber;
                    }
                    else
                    {
                        var orderItem = new OrderItemModel
                        {
                            Quantity = quantityNumber,
                            Coffee = coffee,
                            CoffeeSize = size,
                            OrderItemId = Guid.NewGuid()
                        };
                        cart.OrderItems.Add(orderItem);
                    }
                    Session["cart"] = cart;
                }
                else
                {
                    var cart = new OrderModel();
                    var orderItem = new OrderItemModel
                    {
                        Quantity = quantityNumber,
                        Coffee = coffee,
                        CoffeeSize = size,
                        OrderItemId = Guid.NewGuid()
                    };

                    cart.OrderItems.Add(orderItem);
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
        public ActionResult ChangeCoffeeQuantityFromOrder(string id, string quantity)
        {
            OrderModel cart = (OrderModel)Session["cart"];
            if (cart == null)
            {
                return HttpNotFound();
            }

            var coffeeToRemove = cart.OrderItems.First(item => item.Coffee.CoffeeId == Guid.Parse(id));

            if (coffeeToRemove == null)
                return HttpNotFound();

            coffeeToRemove.Quantity += Convert.ToInt32(quantity);
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

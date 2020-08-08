using CoffeeShop.Models.Order;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CoffeeShop.Helpers
{
    public static class HtmlHelpers
    {
        public static string GetOrderRequestJson(this HtmlHelper helper, OrderModel cart)
        {
            var items = new List<OrderItemModel>();
            if (cart != null)
            {
                items = cart.OrderItems;
            }
            return JsonConvert.SerializeObject(cart, Formatting.None);
        }
    }
}
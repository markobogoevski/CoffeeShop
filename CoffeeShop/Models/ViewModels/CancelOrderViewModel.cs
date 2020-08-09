using System;

namespace CoffeeShop.Models.ViewModels
{
    public class CancelOrderViewModel
    {
        public Guid OrderId { get; set; }

        public bool Cancellable { get; set; }
    }
}
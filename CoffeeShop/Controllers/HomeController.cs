namespace CoffeeShop.Controllers
{
    using CoffeeShop.Services;
    using System;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private Repository _repository;

        public HomeController()
        {
            _repository = Repository.GetInstance();
        }

        public ActionResult Index()
        {
            try
            {
                ViewBag.TopCoffee = _repository.GetTopCoffee();
                return View();
            }
            catch (Exception)
            {
                return new HttpNotFoundResult();
            }
            
        }
    }
}
using CoffeeShop.Models;
using CoffeeShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CoffeeShop.Controllers
{
    [Authorize]
    public class CoffeeController:Controller
    {
        private Repository _repository = Repository.GetInstance();

    }
}
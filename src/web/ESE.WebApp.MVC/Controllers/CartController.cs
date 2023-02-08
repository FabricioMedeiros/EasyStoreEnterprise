using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Controllers
{
    public class CartController : MainController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

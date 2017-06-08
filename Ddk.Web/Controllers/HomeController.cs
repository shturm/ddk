using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ddk.Data;
using Microsoft.EntityFrameworkCore;
using Ddk.Data.Entities;
using Ddk.Web.Models;

namespace Ddk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int carId = 0)
        {
            if (carId != 0)
            {
                ViewData["carId"] = carId;
            }

            IEnumerable<ProductCategory> categories = _db.ProductCategory
                .Where(c => c.ParentId == null)
                .Include("Children.Products")
                .OrderByDescending(c=>c.Products.Count)
                .ToList();
            IEnumerable<string> cars = _db.Car.OrderBy(c => c.Make).Select(c => c.Make).Distinct().ToList();
            var vm = new HomeVM();
            vm.Cars = cars.Select(make => new CarBrandModel() { Make = make }).ToList();
            vm.Categories = categories;

            return View(vm);
        }

        public IActionResult About()
        {
            //ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            //ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

using Ddk.Data;
using Ddk.Data.Entities;
using Ddk.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 100;
            var applicationDbContext = _context.Product.Include(p => p.ProductCategory)
                .Skip(pageSize * (page - 1))
                .Take(pageSize);
            ViewData["CurrentPage"] = page;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET /category/{categoryId}/
        // GET /category/{categoryId}?make=BMW
        // GET /category/{categoryId}?make=BMW&Model=3 Series
        // GET /category/{categoryId}?make=BMW&Model=3 Series&Variant=E46
        //...
        // GET /category/{categoryId}?make=BMW&Model=3 Series&Variant=E46....
        // no year-from/to because will add as a filter feature later on
        [AllowAnonymous]
        public IActionResult PickedProductCategoryChooseCar(
            int categoryId,
            string make = null,
            string model = null, string variant = null, string body = null,
            string engineFuel = null, string type = null, int engineCcm = 0, int engineHp = 0, int engineKw = 0)
        {
            if (make == null)
            {
                return this.ChooseCarMake(categoryId);
            }
            else if (model == null)
            {
                return this.ChooseModelVariantBody(categoryId, make);
            }
            else if (engineFuel == null)
            {
                return this.ChooseEngineType(categoryId, make, model, variant, body);
            }

            ViewData["type"] = type;
            ViewData["engineFuel"] = engineFuel;
            ViewData["engineCcm"] = engineCcm;
            ViewData["engineHp"] = engineHp;
            ViewData["engineKw"] = engineKw;

            // get this when you have all parameters above. Should be only one car
            int carId = _context.Car
                .SingleOrDefault(c =>
                    c.Make == make &&
                    c.Model == model &&
                    c.Variant == variant &&
                    c.Body == body &&
                    c.EngineCcm == engineCcm &&
                    c.EngineFuel == engineFuel &&
                    c.EngineHp == engineHp &&
                    c.EngineKw == engineKw &&
                    c.Type == type)
                .Id;

            // show specific products (of chosen category) compatible with specific car
            return RedirectToAction("CompatibleProducts", new { categoryId, carId });
        }

        // GET /car/{carId}/
        // GET /car/{carId}/category/{categoryId}
        [AllowAnonymous]
        public IActionResult ChooseCar(
            string make = null,
            string model = null, string variant = null, string body = null,
            string engineFuel = null, string type = null, int engineCcm = 0, int engineHp = 0, int engineKw = 0)
        {
            if (make == null)
            {
                return RedirectToAction("Index");
            }
            else if (model == null)
            {
                return this.ChooseModelVariantBody(null, make);
            }
            else if (engineFuel == null)
            {
                return this.ChooseEngineType(null, make, model, variant, body);
            }

            ViewData["type"] = type;
            ViewData["engineFuel"] = engineFuel;
            ViewData["engineCcm"] = engineCcm;
            ViewData["engineHp"] = engineHp;
            ViewData["engineKw"] = engineKw;

            // get this when you have all parameters above. Should be only one car
            int carId = _context.Car
                .SingleOrDefault(c =>
                    c.Make == make &&
                    c.Model == model &&
                    c.Variant == variant &&
                    c.Body == body &&
                    c.EngineCcm == engineCcm &&
                    c.EngineFuel == engineFuel &&
                    c.EngineHp == engineHp &&
                    c.EngineKw == engineKw &&
                    c.Type == type)
                .Id;

            // show specific products (of chosen category) compatible with specific car
            return this.PickedCarChooseProductCategory(carId);
        }

        // GET /car/{carId}/
        // GET /car/{carId}/category/{categoryId}
        [AllowAnonymous]
        public IActionResult PickedCarChooseProductCategory(int carId, int categoryId = 0)
        {
            if (categoryId == 0)
            {
                return RedirectToAction("Index", "Home", new { carId });
            }
            else
            {
                return RedirectToAction("CompatibleProducts", new { categoryId, carId });
            }
        }

        [AllowAnonymous]
        public IActionResult CompatibleProducts(int categoryId, int carId)
        {
            var car = _context.Car.SingleOrDefault(c => c.Id == carId);
            var compatibleProducts = new CompatibleProductsVM();
            compatibleProducts.Category = _context.ProductCategory.SingleOrDefault(pc => pc.Id == categoryId);
            compatibleProducts.Car = new CarVM()
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Variant = car.Variant,
                Body = car.Body,
                Type = car.Type,
                EngineCcm = car.EngineCcm,
                EngineHp = car.EngineHp,
                EngineKw = car.EngineKw,
                EngineFuel = car.EngineFuel
            };
            compatibleProducts.Products = _context.Product
                .Include(p => p.CompatibilitySettings)
                .Where(p => p.ProductCategoryId == categoryId).Where(p =>
                    p.CompatibilitySettings.Any(cs =>
                        (cs.Variant == car.Variant || cs.Variant == "NULL") &&
                        (cs.Model == car.Model || cs.Model == "NULL") &&
                        (cs.Make == car.Make || cs.Make == "NULL")))
                .OrderBy(p => p.Id)
                .Select(p => new ProductVM()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price
                })
                .ToList();

            // select products of {categoryId} which have CompatibilitySetting rows matching car with car ID = {carId}
            return View(compatibleProducts);
        }


        // GET: Products/Details/5
        [AllowAnonymous]
        public IActionResult Details(int? productId, int? carId)
        {
            if (productId == null)
            {
                return NotFound();
            }
            else if (carId == null)
            {
                return NotFound();
            }

            var product = _context.Product
                .Include(p => p.ProductCategory)
                    .ThenInclude(cp => cp.Parent)
                .SingleOrDefault(m => m.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            var car = _context.Car.SingleOrDefault(c => c.Id == carId);

            if (car == null)
            {
                return NotFound();
            }

            var productVM = new ProductDetailsVM()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = new CategoryVM()
                {
                    Id = product.ProductCategory.Id,
                    Name = product.ProductCategory.Name,
                    ParentName = product.ProductCategory.Parent.Name
                },
                Car = new CarVM()
                {
                    Id = car.Id,
                    Make = car.Make,
                    Model = car.Model,
                    Variant = car.Variant,
                    Body = car.Body,
                    Type = car.Type,
                    EngineCcm = car.EngineCcm,
                    EngineHp = car.EngineHp,
                    EngineKw = car.EngineKw,
                    EngineFuel = car.EngineFuel
                }
            };

            return View(productVM);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategory, "Id", "Id");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Name,SKU,OEM,ProductCategoryId,Price,Created,Updated")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategory, "Id", "Id", product.ProductCategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategory, "Id", "Id", product.ProductCategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Name,SKU,OEM,ProductCategoryId,Price,Created,Updated")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategory, "Id", "Id", product.ProductCategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeCategory(int productId, int categoryId, int oldCategoryId)
        {
            var product = _context.Product.Where(x => x.Id == productId).FirstOrDefault();
            if (product == null) NotFound();
            if (!_context.ProductCategory.Any(x => x.Id == categoryId)) NotFound();

            product.ProductCategoryId = categoryId;

            _context.SaveChanges();

            return new RedirectToActionResult("Edit", "ProductCategories", new { id = oldCategoryId });
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductCategory)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        private IActionResult ChooseCarMake(int? categoryId)
        {
            var makes = _context.Car.OrderBy(c => c.Make).Select(x => x.Make).Distinct().ToList();

            var index = 0;
            var matrix = new List<List<string>>();
            foreach (var make in makes)
            {
                if (index % 62 == 0)
                {
                    matrix.Add(new List<string>());
                }

                matrix.Last().Add(make);
                index++;
            }

            if (categoryId == null)
            {
                return View("ChooseMake", matrix);
            }
            else
            {
                ViewData["categoryId"] = categoryId;
                return View("PickedProductCategoryChooseMake", matrix);
            }
        }

        private IActionResult ChooseModelVariantBody(int? categoryId, string make)
        {
            var modelVariantBodyCombinations = _context.Car
                .OrderBy(x => x.Model)
                .ThenBy(x => x.Variant)
                .ThenBy(x => x.Body)
                .Where(x => x.Make == make)
                .GroupBy(x => new
                {
                    x.Model,
                    x.Variant,
                    x.Body
                })
                .Distinct()
                .Select(x => x.First())
                .Select(x =>
                    new ChooseModelVariantBodyVM()
                    {
                        Model = x.Model,
                        Variant = x.Variant,
                        Body = x.Body
                    })
                .AsEnumerable();

            var index = 0;
            var matrix = new List<List<ChooseModelVariantBodyVM>>();
            foreach (var mvb in modelVariantBodyCombinations)
            {
                if (index % 50 == 0)
                {
                    matrix.Add(new List<ChooseModelVariantBodyVM>());
                }

                matrix.Last().Add(mvb);
                index++;
            }

            ViewData["make"] = make;

            if (categoryId == null)
            {
                return View("ChooseModelVariantBody", matrix);
            }
            else
            {
                ViewData["categoryId"] = categoryId;
                return View("PickedProductCategoryChooseModelVariantBody", matrix);
            }
        }

        private IActionResult ChooseEngineType(int? categoryId, string make, string model, string variant, string body)
        {
            IEnumerable<IGrouping<string, ChooseEngineVM>> engineOptions = _context.Car
                .OrderBy(c => c.EngineFuel)
                .ThenBy(c => c.EngineHp)
                .Where(c =>
                    c.Make == make &&
                    c.Model == model &&
                    c.Variant == variant &&
                    c.Body == body)
                .GroupBy(c => new ChooseEngineVM()
                {
                    Ccm = c.EngineCcm,
                    Kw = c.EngineKw,
                    Hp = c.EngineHp,
                    Fuel = c.EngineFuel,
                    Type = c.Type
                })
                .Select(x => x.Key)
                .AsEnumerable()
                .GroupBy(x => x.Fuel);

            ViewData["make"] = make;
            ViewData["model"] = model;
            ViewData["variant"] = variant;
            ViewData["body"] = body;

            if (categoryId == null)
            {
                return View("ChooseEngineType", engineOptions);
            }
            else
            {
                ViewData["categoryId"] = categoryId;
                return View("PickedProductCategoryChooseEngineType", engineOptions);
            }
        }
    }
}

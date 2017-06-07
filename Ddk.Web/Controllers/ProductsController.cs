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
        public IActionResult PickedProductCategoryChooseCar(int categoryId,
                                                            string make = null,
                                                            string model = null, string variant = null, string body = null,
                                                            string engineFuel = null, string type = null, int engineCcm = 0, int engineHp = 0, int engineKw = 0)
        {
            if (make == null)
            {
                List<string> makes = _context.Car.Select(x => x.Make).Distinct().ToList();
                ViewData["categoryId"] = categoryId;

                return View("PickedProductCategoryChooseMake", makes); // view with all makes - https://www.autopower.bg/avtochasti-audi.html
            }

            if (model == null)
            {
                ViewData["categoryId"] = categoryId;
                ViewData["make"] = make;
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
                    .Select(x => new ChooseModelVariantBodyVM()
                    {
                        Model = x.Model,
                        Variant = x.Variant,
                        Body = x.Body
                    })
                    .AsEnumerable();

                return View("PickedProductCategoryChooseModelVariantBody", modelVariantBodyCombinations); // show all disctinct combinations of model/variant/body for current make
            }

            if (engineFuel == null)
            {
                ViewData["categoryId"] = categoryId;
                ViewData["make"] = make;
                ViewData["model"] = model;
                ViewData["variant"] = variant;
                ViewData["body"] = body;
                var typeOptions = _context.Car
                    .OrderBy(c => c.Type)
                    .ThenBy(c => c.EngineFuel)
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
                    .AsEnumerable();

                // show all distinct combinations of type/engineCcm/engineHp/engineKw/engineFuel with current make, model, variant and body
                return View("PickedProductCategoryChooseEngineType", typeOptions);
            }

            ViewData["engineFuel"] = engineFuel;
            ViewData["type"] = type;
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
        public IActionResult ChooseCar(string make = null,
                                       string model = null, string variant = null, string body = null,
                                       string engineFuel = null, string type = null, int engineCcm = 0, int engineHp = 0, int engineKw = 0)
        {
            if (make == null)
            {
                return RedirectToAction("Index");
            }

            if (model == null)
            {
                ViewData["make"] = make;
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
                    .Select(x => new ChooseModelVariantBodyVM()
                    {
                        Model = x.Model,
                        Variant = x.Variant,
                        Body = x.Body
                    })
                    .AsEnumerable();

                // show all disctinct combinations of model/variant/body for current make
                return View("ChooseModelVariantBody", modelVariantBodyCombinations);
            }

            if (engineFuel == null)
            {
                ViewData["make"] = make;
                ViewData["model"] = model;
                ViewData["variant"] = variant;
                ViewData["body"] = body;
                var typeOptions = _context.Car
                    .OrderBy(c => c.Type)
                    .ThenBy(c => c.EngineFuel)
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
                    .AsEnumerable();

                // show all distinct combinations of type/engineCcm/engineHp/engineKw/engineFuel with current make, model, variant and body
                return View("ChooseEngineType", typeOptions);
            }

            ViewData["engineFuel"] = engineFuel;
            ViewData["type"] = type;
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
            return RedirectToAction("PickedCarChooseProductCategory", new { carId });
        }

        // GET /car/{carId}/
        // GET /car/{carId}/category/{categoryId}
        [AllowAnonymous]
        public IActionResult PickedCarChooseProductCategory(int carId, int categoryId = 0)
        {
            var car = _context.Car.SingleOrDefault(c => c.Id == carId);
            ViewData["make"] = car.Make;
            ViewData["model"] = car.Model;
            ViewData["variant"] = car.Variant;
            ViewData["body"] = car.Body;
            ViewData["type"] = car.Type;
            ViewData["engineCcm"] = car.EngineCcm;
            ViewData["engineHp"] = car.EngineHp;
            ViewData["engineKw"] = car.EngineKw;
            ViewData["engineFuel"] = car.EngineFuel;
            if (categoryId == 0)
            {
                return RedirectToAction("Index", "Home", null);
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult CompatibleProducts(int categoryId, int carId)
        {
            var compatibleProducts = new CompatibleProductsVM();
            var car = _context.Car.SingleOrDefault(c => c.Id == carId);
            compatibleProducts.Category = _context.ProductCategory.SingleOrDefault(pc => pc.Id == categoryId);

            compatibleProducts.Car = new CarInformationVM()
            {
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
                .Select(p => new ProductInformationVM()
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price
                })
                .ToList();
            //{
            //    foreach (var cs in p.CompatibilitySettings)
            //    {
            //        if (cs.Variant == "NULL" || cs.Variant == car.Variant)
            //        {
            //            if (cs.Model == "NULL" || cs.Model == car.Model)
            //            {
            //                if (cs.Make == "NULL" || cs.Make == car.Make)
            //                {
            //                    return true;
            //                }
            //            }
            //        }
            //    }

            //    return false;
            //})
            //.ToList();

            return View(compatibleProducts); // select products of {categoryId} which have CompatibilitySetting rows matching car with car ID = {carId}
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public async Task<IActionResult> ChangeCategory(int productId, int categoryId, int oldCategoryId)
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
    }
}

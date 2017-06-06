using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ddk.Data;
using Ddk.Data.Entities;
using Microsoft.AspNetCore.Authorization;

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
                .Skip(pageSize * (page-1))
                .Take(pageSize);
            ViewData["CurrentPage"] = page;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET /products/{categoryId}/
        // GET /products/{categoryId}?make=BMW
        // GET /products/{categoryId}?make=BMW&Model=3 Series
        // GET /products/{categoryId}?make=BMW&Model=3 Series&Variant=E46
        //...
        // GET /products/{categoryId}?make=BMW&Model=3 Series&Variant=E46....
        // no year-from/to because will add as a filter feature later on
        public IActionResult PickedProductCategoryChooseCar(int categoryId,
                                                            string make = null, 
                                                            string model = null, string variant = null, string body = null, 
                                                            string type = null, int engineCcm = 0, int engineHp = 0, int engineKw = 0, string engineFuel = null)
        {
            
            if (make == null)
            {
                return View(); // view with all makes - https://www.autopower.bg/avtochasti-audi.html
            }

            if (model == null)
            {
                return View(); // show all disctinct combinations of model/variant/body for current make
            }

            if (type == null)
            {
                return View(); // show all distinct combinations of type/engineCcm/engineHp/engineKw/engineFuel with current make, model, variant and body
            }

            int carId = 0; // get this when you have all parameters above. Should be only one car
            return RedirectToAction("CompatibleProducts", new { categoryId, carId }); // show specific products (of chosen category) compatible with specific car
        }

        // GET /car/{carId}/
        // GET /car/{carId}/category/{categoryId}
        public IActionResult PickedCarChooseProductCategory(int carId, int categoryId = 0)
        {
            if (categoryId == 0)
            {
                return View(); // show all categories to choose from
            }

            return RedirectToAction("CompatibleProducts", new { categoryId, carId}); // show specific products (of chosen category) compatible with specific car
        }

        public IActionResult CompatibleProducts(int categoryId, int carId)
        {
            return View(); // select products of {categoryId} which have CompatibilitySetting rows matching car with car ID = {carId}
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

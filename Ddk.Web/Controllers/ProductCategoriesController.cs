using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ddk.Data;
using Ddk.Data.Entities;
using Ddk.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ddk.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoriesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: ProductCategories
        public IActionResult Index(int page = 1)
        {
            return RedirectToAction("Index", "Home");

            int pageSize = 100;
            var model = (from c in _context.ProductCategory
                       select new ProductCategoryVM
                       {
                           Id = c.Id,
                           Name = c.Name,
                           ParentName = c.Parent.Name,
                           ParentId = c.ParentId,
                           ChildrenCount = c.Children.Count,
                           ProductsCount = c.Products.Count
                       }).Skip(pageSize * (page - 1))
                         .Take(pageSize)
                         .ToList();
            ViewData["CurrentPage"] = page;
            return View(model);
        }

        // GET: ProductCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory
                .Include(p => p.Parent)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: ProductCategories/Create
        public IActionResult Create(int parentId)
        {
            var parent = _context.ProductCategory.Find(parentId);
            if (parent == null) NotFound($"Няма такава родителска категория ID#{parentId}");
            var model = new ProductCategoryVM(parent);
            return View(model);
        }

        public IActionResult CreateRoot()
        {
            ViewData["ParentId"] = new SelectList(_context.ProductCategory.Where(pc=>pc.ParentId == null), "Id", "Name");
            return View();
        }

        // POST: ProductCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ParentId,Created,Updated")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", new { id=productCategory.ParentId});
            }
            ViewData["ParentId"] = new SelectList(_context.ProductCategory.Where(pc => pc.ParentId == null), "Id", "Name", productCategory.ParentId);
            return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // premature ToList() and double mapping is necessary: https://github.com/aspnet/EntityFramework/issues/7714#issuecomment-286828265
            var subCategories = _context.ProductCategory
                .Where(sc => sc.ParentId == id)
                .Select(sc=> new {
                    Id = sc.Id,
                    Name = sc.Name,
                    ProductsCount = sc.Products.Count
                }).ToList()
                .Select(sc=> new SubCategoryVM()
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    ProductsCount = sc.ProductsCount
                });
            ProductCategoryEditVM model = _context.ProductCategory
                .Where(m => m.Id == id)
                .Select(pc => new ProductCategoryEditVM()
                {
                   Id = pc.Id,
                   ChildrenCount = pc.Children.Count,
                   Children = subCategories,
                   ProductsCount = pc.Products.Count,
                   Products = pc.Products,
                   Name = pc.Name,
                   ParentName = pc.Parent.Name,
                   ParentId = pc.ParentId
                })
                .FirstOrDefault();
            if (model == null)
            {
                return NotFound();
            }
            var productCategories = _context.ProductCategory
                .Include(pc=>pc.Parent)
                .Where(pc => pc.ParentId != null)
                .OrderBy(pc=>pc.Name)
                .ToList();
            var rootCategories = _context.ProductCategory
                .Include(pc => pc.Parent)
                .Where(pc => pc.ParentId == null)
                .OrderBy(pc => pc.Name)
                .ToList();
            ViewData["ProductCategories"] = new SelectList(productCategories, "Id", "Name", model.Id, "Parent.Name");
            ViewData["RootCategoriesForCurrentCategory"] = new SelectList(rootCategories, "Id", "Name", model.ParentId);
            ViewData["RootCategoriesForChildrenOfRootCateogory"] = new SelectList(rootCategories, "Id", "Name", model.Id);


            return View(model);
        }

        public async Task<IActionResult> ChangeParentCategory (int childId, int parentId, int oldParentId)
        {
            var child = _context.ProductCategory.Where(x => x.Id == childId).FirstOrDefault();
            if (!_context.ProductCategory.Any(x => x.Id == parentId)) NotFound();
            if (child == null) NotFound();

            child.ParentId = parentId;
            _context.SaveChanges();

            return RedirectToAction("Edit", new { id = oldParentId });
        }

        // POST: ProductCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ParentId,Created,Updated")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
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
            ViewData["ParentId"] = new SelectList(_context.ProductCategory, "Id", "Id", productCategory.ParentId);
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory
                .Include(p => p.Parent)
                .Include(p=>p.Children)
                .Include(p=>p.Products)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            if (productCategory.Children.Count > 0 || productCategory.Products.Count > 0)
            {
                ViewData["Message"] = $"За да се изтрие категорията от нея трябва да се преместят всички продукти и под-категории. В {productCategory.Name} има {productCategory.Children.Count} под-категории и {productCategory.Products.Count} продукта";
                return View("Message");
            }

            return View(productCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productCategory = await _context.ProductCategory.SingleOrDefaultAsync(m => m.Id == id);
            _context.ProductCategory.Remove(productCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategory.Any(e => e.Id == id);
        }
    }
}

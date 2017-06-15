using Ddk.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Web.ViewComponents
{
    public class CategoriesListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoriesListViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int carId)
        {
            ViewData["carId"] = carId;
            var car = _context.Car.SingleOrDefault(c => c.Id == carId);
            var categories = await _context.ProductCategory
                .Where(c => c.ParentId == null)
                .Include(c => c.Children)
                    .ThenInclude(c => c.Products)
                .OrderByDescending(c => c.Products.Count)
                .ToListAsync();
            return View(categories);
        }
    }
}

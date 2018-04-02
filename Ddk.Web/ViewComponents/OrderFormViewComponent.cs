using Ddk.Data;
using Ddk.Web.Models.AccountViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ddk.Web.ViewComponents
{
    public class OrderFormViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public OrderFormViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var product = await _context.Product.SingleOrDefaultAsync(p => p.Id == productId);
            ViewData["productId"] = product.Id;
            ViewData["productName"] = product.Name;
            ViewData["productDescription"] = product.Description;
            ViewData["productPrice"] = product.Price;
            ViewData["productQuantity"] = 1;

            var deliveryData = new DeliveryDataViewModel();
            var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (dbUser != null)
            {
                deliveryData = new DeliveryDataViewModel()
                {
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    PhoneNumber = dbUser.PhoneNumber,
                    Address = dbUser.Address,
                    City = dbUser.City,
                    CompanyName = dbUser.CompanyName,
                    CompanyEIK = dbUser.CompanyEIK,
                    Tax = dbUser.Tax
                };
            }

            return View(deliveryData);
        }
    }
}

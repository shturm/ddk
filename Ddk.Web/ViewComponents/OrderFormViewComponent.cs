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
            ViewData["productId"] = productId;
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
                    City = dbUser.City
                };
            }

            return View(deliveryData);
        }
    }
}

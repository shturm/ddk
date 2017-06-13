using Ddk.Data;
using Ddk.Web.Data.Entities;
using Ddk.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Ddk.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Order.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .SingleOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.IsAvailable)
            {
                var objectsAsString = HttpContext.Session.GetString("orderItems");

                var orderItems = JsonConvert.DeserializeObject<List<OrderItemVM>>(objectsAsString);
                foreach (var orderItem in orderItems)
                {
                    var product = _context.Product.SingleOrDefault(p => p.Id == orderItem.ProductId);
                    if (product != null)
                    {
                        orderItem.Name = product.Name;
                        orderItem.Description = product.Description;
                        orderItem.Price = product.Price;
                        orderItem.ImageUrl = product.ImageUrl;
                    }
                }

                var orderVm = new OrderVM() { Products = orderItems };
                return View(orderVm);
            }


            return NotFound();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Create(OrderVM orderVM)
        {
            if (ModelState.IsValid)
            {
                var order = new Order()
                {
                    Status = OrderStatus.Pending,
                    Names = orderVM.Names,
                    PhoneNumber = orderVM.PhoneNumber,
                    Address = orderVM.Address,
                    City = orderVM.City,
                    MoreInformation = orderVM.MoreInformation,
                    CompanyName = orderVM.CompanyName,
                    CompanyEIK = orderVM.CompanyEIK,
                    Created = DateTime.Now,
                    Items = orderVM.Products.Select(p => new OrderItem()
                    {
                        ProductId = p.Id,
                        Description = p.Description,
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToList()
                };

                _context.Order.Add(order);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orderVM);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.SingleOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .SingleOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.SingleOrDefaultAsync(m => m.Id == id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public void AddToBasket(int productId, int quantity)
        {
            if (!_context.Product.Any(p => p.Id == productId))
            {
                return;
            }

            var key = "orderItems";
            var objectsCollection = new List<OrderItemVM>();
            if (HttpContext.Session.IsExists(key))
            {
                var objectsAsString = HttpContext.Session.GetString(key);
                objectsCollection = JsonConvert.DeserializeObject<List<OrderItemVM>>(objectsAsString);
            }


            objectsCollection.Add(new OrderItemVM() { ProductId = productId, Quantity = quantity });

            var result = JsonConvert.SerializeObject(objectsCollection);

            HttpContext.Session.SetString(key, result);
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}

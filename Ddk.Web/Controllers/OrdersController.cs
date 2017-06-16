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
        private const string OrderItemsSessionDictionaryKey = "OrderItems";

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        [HttpGet]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminIndex", "Orders");
            }
            else
            {
                return RedirectToAction("UserIndex", "Orders");
            }
        }

        public IActionResult AdminIndex()
        {
            var orders = _context.Order
                    .Select(o => new OrderVM()
                    {
                        Id = o.Id,
                        Status = o.Status,
                        Names = o.Names,
                        PhoneNumber = o.PhoneNumber,
                        Address = o.Address,
                        City = o.City,
                        MoreInformation = o.MoreInformation,
                        CompanyName = o.CompanyName,
                        CompanyEIK = o.CompanyEIK,
                        Created = o.Created,
                        OrderItems = o.Items.Select(p => new OrderItemVM()
                        {
                            ProductId = p.Id,
                            Description = p.Description,
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = p.Quantity
                        }).AsEnumerable()
                    }).ToList();
            return View(orders);
        }

        public IActionResult UserIndex()
        {
            var userId = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name).Id;
            var orders = _context.Order
                  .Where(o => o.UserId == userId)
                  .Select(o => new OrderVM()
                  {
                      Id = o.Id,
                      Status = o.Status,
                      Names = o.Names,
                      PhoneNumber = o.PhoneNumber,
                      Address = o.Address,
                      City = o.City,
                      MoreInformation = o.MoreInformation,
                      CompanyName = o.CompanyName,
                      CompanyEIK = o.CompanyEIK,
                      Created = o.Created,
                      OrderItems = o.Items.Select(p => new OrderItemVM()
                      {
                          ProductId = p.ProductId,
                          Description = p.Description,
                          Name = p.Name,
                          Price = p.Price,
                          Quantity = p.Quantity
                      }).AsEnumerable()
                  })
                  .ToList();
            return View(orders);
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

            var orderVm = new OrderVM()
            {
                Id = order.Id,
                Status = order.Status,
                Names = order.Names,
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                City = order.City,
                MoreInformation = order.MoreInformation,
                CompanyName = order.CompanyName,
                CompanyEIK = order.CompanyEIK,
                Created = order.Created,
                OrderItems = order.Items.Select(p => new OrderItemVM()
                {
                    ProductId = p.Id,
                    Description = p.Description,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity
                }).AsEnumerable()
            };

            return View(orderVm);
        }

        // GET: Orders/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.IsAvailable)
            {
                var objectsAsString = HttpContext.Session.GetString(OrderItemsSessionDictionaryKey);
                if (string.IsNullOrEmpty(objectsAsString))
                {
                    return View("EmptyBasket");
                }

                var orderItems = JsonConvert.DeserializeObject<List<OrderItemVM>>(objectsAsString);
                if (orderItems.Count() == 0)
                {
                    return View("EmptyBasket");
                }

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

                var order = new OrderVM() { OrderItems = orderItems };
                if (User.Identity.IsAuthenticated)
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                    order.UserId = user.Id;
                    order.Names = user.FirstName + " " + user.LastName;
                    order.PhoneNumber = user.PhoneNumber;
                    order.Address = user.Address;
                    order.City = user.City;
                    order.CompanyName = user.CompanyName;
                    order.CompanyEIK = user.CompanyEIK;
                }

                return View(order);
            }

            return NotFound();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
                    Items = orderVM.OrderItems.Select(p => new OrderItem()
                    {
                        ProductId = p.Id,
                        Description = p.Description,
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = p.Quantity
                    })
                    .ToList()
                };

                if (User.Identity.IsAuthenticated)
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                    order.UserId = user.Id;
                }

                _context.Order.Add(order);
                _context.SaveChanges();
                
                var result = JsonConvert.SerializeObject(new List<OrderItemVM>());
                HttpContext.Session.SetString(OrderItemsSessionDictionaryKey, result);

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
            
            var orderItems = new List<OrderItemVM>();
            if (HttpContext.Session.IsExists(OrderItemsSessionDictionaryKey))
            {
                var objectsAsString = HttpContext.Session.GetString(OrderItemsSessionDictionaryKey);
                orderItems = JsonConvert.DeserializeObject<List<OrderItemVM>>(objectsAsString);
            }

            if (orderItems.Any(x => x.ProductId == productId))
            {
                var orderItem = orderItems.SingleOrDefault(or => or.ProductId == productId);
                orderItem.Quantity += quantity;
            }
            else
            {
                orderItems.Add(new OrderItemVM() { ProductId = productId, Quantity = quantity });
            }

            var result = JsonConvert.SerializeObject(orderItems);

            HttpContext.Session.SetString(OrderItemsSessionDictionaryKey, result);
        }

        public void RemoveItemFromBasket(int productId)
        {
            if (HttpContext.Session.IsAvailable)
            {
                var objectsAsString = HttpContext.Session.GetString(OrderItemsSessionDictionaryKey);

                var orderItems = JsonConvert.DeserializeObject<List<OrderItemVM>>(objectsAsString);

                var itemToRemove = orderItems.SingleOrDefault(i => i.ProductId == productId);
                if (itemToRemove != null)
                {
                    orderItems.Remove(itemToRemove);
                }

                var result = JsonConvert.SerializeObject(orderItems);

                HttpContext.Session.SetString(OrderItemsSessionDictionaryKey, result);
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}

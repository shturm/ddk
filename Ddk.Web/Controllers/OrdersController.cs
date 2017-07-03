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
using Ddk.Data.Entities;
using Ddk.Web.Services;

namespace Ddk.Web.Controllers
{
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

        [Authorize(Roles = "Admin")]
        public IActionResult AdminIndex()
        {
            var orders = _context.Order
                    .Select(o => new OrderVM()
                    {
                        Id = o.Id,
                        Status = o.Status,
                        Names = o.Names,
                        Email = o.Email,
                        PhoneNumber = o.PhoneNumber,
                        Address = o.Address,
                        City = o.City,
                        MoreInformation = o.MoreInformation,
                        CompanyName = o.CompanyName,
                        CompanyEIK = o.CompanyEIK,
                        Created = o.Created,
                        Tax = o.Tax,
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

        [Authorize]
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
                      Email = o.Email,
                      PhoneNumber = o.PhoneNumber,
                      Address = o.Address,
                      City = o.City,
                      MoreInformation = o.MoreInformation,
                      CompanyName = o.CompanyName,
                      CompanyEIK = o.CompanyEIK,
                      Tax = o.Tax,
                      Created = o.Created,
                      OrderItems = o.Items.Select(or => new OrderItemVM()
                      {
                          ProductId = or.ProductId,
                          Description = or.Description,
                          Name = or.Name,
                          Price = or.Price,
                          Quantity = or.Quantity
                      }).AsEnumerable()
                  })
                  .ToList();
            return View(orders);
        }

        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(x=>x.Items)
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
                Email = order.Email,
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                City = order.City,
                MoreInformation = order.MoreInformation,
                CompanyName = order.CompanyName,
                CompanyEIK = order.CompanyEIK,
                Tax = order.Tax,
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
                    order.Tax = user.Tax;
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
                    Email = orderVM.Email,
                    PhoneNumber = orderVM.PhoneNumber,
                    Address = orderVM.Address,
                    City = orderVM.City,
                    MoreInformation = orderVM.MoreInformation,
                    CompanyName = orderVM.CompanyName,
                    CompanyEIK = orderVM.CompanyEIK,
                    Tax = orderVM.Tax,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Items = orderVM.OrderItems.Select(p => new OrderItem()
                    {
                        ProductId = p.ProductId,
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

                this.SendMessageToAdmins(order);
                return View("SuccessfulОrder");
            }

            return View(orderVM);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderVM = _context.Order
                   .Select(o => new OrderVM()
                   {
                       Id = o.Id,
                       Status = o.Status,
                       Names = o.Names,
                       Email = o.Email,
                       PhoneNumber = o.PhoneNumber,
                       Address = o.Address,
                       City = o.City,
                       MoreInformation = o.MoreInformation,
                       CompanyName = o.CompanyName,
                       CompanyEIK = o.CompanyEIK,
                       Tax = o.Tax,
                       Created = o.Created,
                       OrderItems = o.Items.Select(or => new OrderItemVM()
                       {
                           Id = or.Id,
                           ProductId = or.ProductId,
                           Description = or.Description,
                           Name = or.Name,
                           Price = or.Price,
                           Quantity = or.Quantity,
                           ImageUrl = _context.Product
                            .SingleOrDefault(p => p.Id == or.ProductId)
                            .ImageUrl
                       })
                       .ToList()
                   })
                   .SingleOrDefault(o => o.Id == id);

            if (orderVM == null)
            {
                return NotFound();
            }

            return View(orderVM);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditOrderVM orderVM)
        {
            //If / ModelState.IsValid dose not work
            try
            {
                var order = _context.Order
                    .Include(o => o.Items)
                    .SingleOrDefault(o => o.Id == orderVM.Id);
                order.Names = orderVM.Names;
                order.Email = orderVM.Email;
                order.PhoneNumber = orderVM.PhoneNumber;
                order.Address = orderVM.Address;
                order.City = orderVM.City;
                order.CompanyName = orderVM.CompanyName;
                order.CompanyEIK = orderVM.CompanyEIK;
                order.Tax = orderVM.Tax;
                order.Updated = DateTime.Now;
                order.Status = orderVM.Status;

                var removedOrderItems = order.Items
                    .Where(x => !orderVM.OrderItems.Any(y => y.Id == x.Id))
                    .ToList();

                foreach (var orderItem in removedOrderItems)
                {
                    order.Items.Remove(orderItem);
                }

                foreach (var orderItemVM in orderVM.OrderItems)
                {
                    var orderItem = order.Items.SingleOrDefault(or => or.Id == orderItemVM.Id);
                    if (orderItem == null)
                    {
                        var product = _context.Product.SingleOrDefault(p => p.Id == orderItemVM.ProductId);
                        if (product == null)
                        {
                            product = new Product()
                            {
                                Name = orderItemVM.Name,
                                CompatibilitySettings = new List<CompatibilitySetting>()
                            };
                            _context.Add(product);
                            //throw error
                            //_context.SaveChanges();
                        }
                        else
                        {
                            orderItem = new OrderItem()
                            {
                                ProductId = product.Id,
                                Name = product.Name,
                                Description = product.Description,
                                Price = product.Price,
                                Quantity = orderItemVM.Quantity
                            };
                            order.Items.Add(orderItem);
                        }
                    }
                    else
                    {
                        orderItem.Quantity = orderItemVM.Quantity;
                    }
                }

                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(orderVM.Id))
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

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        private void SendMessageToAdmins(Order order)
        {
            var subject = $"Поръчка №{order.Id}";
            var message =
                $"Здравейте, \r\n" +
                $"Направена е поръчка номер: {order.Id} \r\n";

            foreach (var orderItem in order.Items)
            {
                message += $"{orderItem.Name}, Цена: {orderItem.Price}, Броя: {orderItem.Quantity}";
            }

            var sum = order.Items.Select(i => i.Price * i.Quantity).Sum();
            message += $" Общо: {sum} лв. ДДС: {order.Tax} лв. \r\n";
            message += $"Поздрави";
            message += $" DaiDaKaram.com";
            
            var emailSender = new EmailSender();
            emailSender.SendEmail(subject, message);
        }
    }
}

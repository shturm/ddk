using Ddk.Web.Data.Entities;
using System;
using System.Collections.Generic;

namespace Ddk.Web.Models
{
    public class EditOrderVM
    {
        public EditOrderVM()
        {
            this.OrderItems = new List<OrderItemVM>();
        }

        public int Id { get; set; }
        
        public string Names { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }
        
        //public string AdminNote { get; set; }

        public string CompanyName { get; set; }

        public string CompanyEIK { get; set; }

        public string Tax { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public IEnumerable<OrderItemVM> OrderItems { get; set; }

        public OrderStatus Status { get; set; }
    }
}

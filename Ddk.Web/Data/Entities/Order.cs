using System;
using System.Collections.Generic;

namespace Ddk.Web.Data.Entities
{
    public class Order
    {
        public Order()
        {
            this.Items = new List<OrderItem>();
        }

        public int Id { get; set; }

        public string UserId { get; set; }

        public string Names { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string MoreInformation { get; set; }

        public string AdminNote { get; set; }
        
        public string CompanyName { get; set; }

        public string CompanyEIK { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public ICollection<OrderItem> Items { get; set; }

        public OrderStatus Status { get; set; }
    }
}

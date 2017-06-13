using System.Collections.Generic;

namespace Ddk.Web.Models
{
    public class OrderVM
    {
        public OrderVM()
        {
            this.Products = new List<OrderItemVM>();
        }

        public int Id { get; set; }

        public string Names { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string MoreInformation { get; set; }

        public string AdminNote { get; set; }

        public string CompanyName { get; set; }

        public string CompanyEIK { get; set; }

        public ICollection<OrderItemVM> Products { get; set; }
    }
}

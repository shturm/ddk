using Ddk.Data.Entities;
using System.Collections.Generic;

namespace Ddk.Web.Models
{
    public class CompatibleProductsVM
    {
        public CompatibleProductsVM()
        {
            this.Products = new List<ProductVM>();
        }

        public CarVM Car { get; set; }

        public ProductCategory Category { get; set; }

        public List<ProductVM> Products { get; set; }
    }
}

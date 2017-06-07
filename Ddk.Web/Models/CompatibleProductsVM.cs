using Ddk.Data.Entities;
using System.Collections.Generic;

namespace Ddk.Web.Models
{
    public class CompatibleProductsVM
    {
        public CompatibleProductsVM()
        {
            this.Products = new List<ProductInformationVM>();
        }

        public CarInformationVM Car { get; set; }

        public ProductCategory Category { get; set; }

        public List<ProductInformationVM> Products { get; set; }
    }
}

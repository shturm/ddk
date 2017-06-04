using Ddk.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Web.Models
{
    public class ProductCategoryEditVM : ProductCategoryVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<SubCategoryVM> Children { get; set; }
    }
}

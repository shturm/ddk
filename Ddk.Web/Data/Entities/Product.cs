using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string OEM { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public ICollection<CompatibilitySetting> CompatibilitySettings { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Data.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public ICollection<Product> Products { get; set; }
        [ForeignKey("ParentId")]
        public ICollection<ProductCategory> Children { get; set; }
        public ProductCategory Parent { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public ProductCategory()
        {
            Products = new List<Product>();
            Children = new List<ProductCategory>();
        }
    }
}

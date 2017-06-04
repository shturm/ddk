using Ddk.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Web.Models
{
    public class ProductCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChildrenCount { get; set; }
        public int? ProductsCount { get; set; }
        public string ParentName { get; set; }
        public int? ParentId { get; set; }

        public ProductCategoryVM(ProductCategory pc)
        {
            Id = pc.Id;
            Name = pc.Name;
            ChildrenCount = pc.Children?.Count;
            ProductsCount = pc.Products?.Count;
            ParentId = pc.Parent?.Id;
            ParentName = pc.Parent?.Name;
        }

        public ProductCategoryVM()
        {

        }
    }
}

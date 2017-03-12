using Ddk.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Web.Models
{
    public class HomeVM
    {
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<CarBrandModel> Cars { get; set; }
    }
}

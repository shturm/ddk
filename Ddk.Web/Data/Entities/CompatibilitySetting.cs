using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Data.Entities
{
    public class CompatibilitySetting
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Variant { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}

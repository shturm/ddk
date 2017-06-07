using Ddk.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Web.Models
{
    public class ChooseCarVm
    {
        public int CategoryId { get; set; }
        public List<Car> Cars { get; set; }
    }
}

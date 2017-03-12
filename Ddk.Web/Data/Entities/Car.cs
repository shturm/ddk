using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ddk.Data.Entities
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Variant { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public int EngineCcm { get; set; }
        public int EngineHp { get; set; }
        public int EngineKw { get; set; }
        public string EngineFuel { get; set; }
        public int KType { get; set; }
    }
}

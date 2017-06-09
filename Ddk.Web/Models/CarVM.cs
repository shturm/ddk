namespace Ddk.Web.Models
{
    public class CarVM
    {
        public int Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Variant { get; set; }

        public string Body { get; set; }
        
        public string Type { get; set; }
        
        public int EngineCcm { get; set; }

        public int EngineHp { get; set; }
        
        public int EngineKw { get; set; }

        public string EngineFuel { get; set; }

        public override string ToString()
        {
            return string.Concat(Make, " ", Model, " ", Variant, " ", Body, " ", 
                Type, " ", "( ", EngineFuel, ", куб.см. ", EngineCcm, ", к.с. ", EngineHp, ", кВ ", EngineKw, ", "," )");
        }
    }
}

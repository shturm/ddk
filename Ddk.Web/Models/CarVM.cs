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

        public int YearFrom { get; set; }

        public int YearTo { get; set; }

        public int EngineCcm { get; set; }

        public int EngineHp { get; set; }
        
        public int EngineKw { get; set; }

        public string EngineFuel { get; set; }

        public override string ToString()
        {
            return $"{Make} {Model} {Variant} {Body} {Type} ({EngineFuel}, {EngineCcm} куб.см., {EngineHp} к.с., {EngineKw} кВ, от: {YearFrom}г. до: {YearTo}г.)";
        }
    }
}

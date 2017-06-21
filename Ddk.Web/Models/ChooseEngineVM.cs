namespace Ddk.Web.Models
{
    public class ChooseEngineVM
    {
        public int Ccm { get; set; }

        public int Hp { get; set; }

        public int Kw { get; set; }

        public string Type { get; set; }

        public string Fuel { get; set; }

        public int YearFrom { get; set; }

        public int YearTo { get; set; }

        public override string ToString()
        {
            return $"{Fuel} {Type}, {Ccm} куб.см., {Hp} к.с., {Kw} кВ, {YearFrom} г. - {YearTo} г.";
        }
    }
}

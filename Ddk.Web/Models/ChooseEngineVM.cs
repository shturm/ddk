namespace Ddk.Web.Models
{
    public class ChooseEngineVM
    {
        public int Ccm { get; set; }

        public int Hp { get; set; }

        public int Kw { get; set; }

        public string Type { get; set; }

        public string Fuel { get; set; }

        public override string ToString()
        {
            return string.Concat(Fuel, " ", Type, " ", Ccm, " ", Hp, " ", Kw);
        }
    }
}

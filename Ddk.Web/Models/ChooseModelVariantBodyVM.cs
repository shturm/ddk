namespace Ddk.Web.Models
{
    public class ChooseModelVariantBodyVM
    {
        public string Model { get; set; }

        public string Variant { get; set; }

        public string Body { get; set; }

        public override string ToString()
        {
            return string.Concat(Model, ", ", Variant, ", ", Body);
        }
    }
}

namespace Ddk.Web.Models
{
    public class ProductInformationVM
    {
        public string Name { get; set; }

        public string Description { get; set; }
        
        public decimal Price { get; set; }

        public override string ToString()
        {
            return string.Concat(Name);
        }
    }
}

namespace Ddk.Web.Models
{
    public class ProductDetailsVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string CarInformation { get; set; }

        public string CategoryName { get; set; }

        public override string ToString()
        {
            return string.Concat(Name);
        }
    }
}

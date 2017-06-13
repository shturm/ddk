namespace Ddk.Web.Models
{
    public class OrderItemVM
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int Quantity { get; set; }
    }
}

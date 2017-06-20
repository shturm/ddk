using Ddk.Data.Entities;

namespace Ddk.Web.Data.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
        
        public decimal Price { get; set; }    

        public int Quantity { get; set; }
    }
}

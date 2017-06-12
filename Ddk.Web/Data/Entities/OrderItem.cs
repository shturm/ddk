namespace Ddk.Web.Data.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
        
        public decimal Price { get; set; }    
    }
}

namespace OA.Domain.Entities
{
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Order Orders { get; set; }
        public Product Product { get; set; }
    }
}

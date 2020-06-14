namespace OnionArchitecture.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int ProductId { get; set; }
        public Order Orders { get; set; }
        public Product Product { get; set; }
    }
}

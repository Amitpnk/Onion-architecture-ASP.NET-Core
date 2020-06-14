namespace OnionArchitecture.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public Order Orders { get; set; }
        public Product Product { get; set; }
    }
}

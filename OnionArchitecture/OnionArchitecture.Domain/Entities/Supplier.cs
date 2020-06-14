using System.Collections.Generic;

namespace OnionArchitecture.Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public List<Product> Products { get; set; }
    }
}

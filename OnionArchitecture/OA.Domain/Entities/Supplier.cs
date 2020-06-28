using System.Collections.Generic;

namespace OA.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public string SupplierName { get; set; }
        public List<Product> Products { get; set; }
    }
}

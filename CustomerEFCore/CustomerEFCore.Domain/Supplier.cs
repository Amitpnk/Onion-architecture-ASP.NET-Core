using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerEFCore.Domain
{
    public class Supplier
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public List<Product> Products { get; set; }
    }
}

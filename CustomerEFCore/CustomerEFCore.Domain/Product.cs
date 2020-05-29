using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerEFCore.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

    }
}

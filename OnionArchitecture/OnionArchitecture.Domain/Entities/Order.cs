using System;
using System.Collections.Generic;

namespace OnionArchitecture.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Customer Customers { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

    }
}

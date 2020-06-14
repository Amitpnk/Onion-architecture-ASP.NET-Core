using System.ComponentModel.DataAnnotations;

namespace OnionArchitecture.Domain
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace OA.Domain
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}

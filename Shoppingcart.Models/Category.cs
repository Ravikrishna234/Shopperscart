using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Shoppingcart.Models
{
    public class Category
    {
        [Key] // Primary Key
        public int Id { get; set; }
        [Required] // have not null setting
        [MaxLength(30)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}

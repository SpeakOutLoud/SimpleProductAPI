using System.ComponentModel.DataAnnotations;

namespace SimpleProductAPI.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }

    public class ProductDto
    {
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is Required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is Required")]
        public int Quantity { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RestaurantApi.Models
{
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Restaurant_Id { get; set; }

        [Required(ErrorMessage = "Restaurant name is required")]
        [MaxLength(255)]
        public string Restaurant_Name { get; set; }

        [MaxLength(255)]
        public string Restaurant_Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        [RegularExpression(@"^[0-9+\-]+$", ErrorMessage = "Phone number can contain only digits and symbols '+' and '-'")]
        public string Restaurant_Phone { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public decimal Rating { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [MaxLength(100)]
        public string Category { get; set; }

        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; }

        [JsonIgnore]
        public ICollection<Dish>? Dishes { get; set; }
    }
}

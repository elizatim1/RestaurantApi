using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RestaurantApi.Models
{
    public class Dish
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Dish_Id { get; set; }

        [Required(ErrorMessage = "Dish name is required")]
        [MaxLength(255)]
        public string Dish_Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [MaxLength(100)]
        public string Category { get; set; }

        [Required(ErrorMessage = "Restaurant is required")]
        [ForeignKey("Restaurant")]
        public int Restaurant_Id { get; set; }

        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }

        [JsonIgnore]
        public ICollection<OrderDish>? OrderDishes { get; set; }
    }
}

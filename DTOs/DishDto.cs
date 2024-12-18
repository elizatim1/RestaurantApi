using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.DTOs
{
    public class DishDto
    {
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
        public int Restaurant_Id { get; set; }
    }
}
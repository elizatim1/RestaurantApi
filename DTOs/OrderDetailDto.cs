using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.DTOs
{
    public class OrderDetailDto
    {
        [Required]
        public int Dish_Id { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}

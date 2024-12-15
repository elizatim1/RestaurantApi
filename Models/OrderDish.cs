using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RestaurantApi.Models
{
    public class OrderDish
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Order")]
        public int Order_Id { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Dish")]
        public int Dish_Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }

        [JsonIgnore]
        public Dish? Dish { get; set; }
    }
}

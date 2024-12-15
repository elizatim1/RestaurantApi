using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RestaurantApi.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Order_Id { get; set; }

        [Required(ErrorMessage = "User is required")]
        public int User_Id { get; set; }

        [Required(ErrorMessage = "Restaurant is required")]
        public int Restaurant_Id { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [DataType(DataType.DateTime)]
        public DateTime Order_Date { get; set; }

        [Required(ErrorMessage = "Delivery address is required")]
        [MaxLength(255)]
        public string Delivery_Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }

        [JsonIgnore]
        public ICollection<OrderDish>? OrderDishes { get; set; }
    }
}

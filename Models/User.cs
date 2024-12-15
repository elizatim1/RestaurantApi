using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace RestaurantApi.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int User_Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(255)]
        public string First_Name { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(255)]
        public string Last_Name { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(255)]
        public string Username { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [MaxLength(255)]
        public string Password_Hash { get; set; }

        [MaxLength(255)]
        public string User_Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        [RegularExpression(@"^[0-9+\-]+$", ErrorMessage = "Phone number can contain only digits and symbols '+' and '-'")]
        public string User_Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100)]
        public string Email { get; set; }

        [ForeignKey("Role")]
        public int Role_Id { get; set; }

        [JsonIgnore]
        public Role? Role { get; set; }

        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; }
    }
}

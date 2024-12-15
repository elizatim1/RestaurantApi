using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.DTOs
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(255)]
        public string First_Name { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(255)]
        public string Last_Name { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(255)]
        public string Username { get; set; }

        [MaxLength(255)]
        public string User_Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        [RegularExpression(@"^[0-9+\-]+$", ErrorMessage = "Phone number can contain only digits and symbols '+' and '-'")]
        public string User_Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public int Role_Id { get; set; }
    }
}
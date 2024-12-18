using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.DTOs
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "User is required")]
        public int User_Id { get; set; }

        [Required(ErrorMessage = "Restaurant is required")]
        public int Restaurant_Id { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [DataType(DataType.DateTime)]
        public DateTime Order_Date { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required(ErrorMessage = "Delivery address is required")]
        [MaxLength(255)]
        public string Delivery_Address { get; set; }

        [Required(ErrorMessage = "Order details are required")]
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}

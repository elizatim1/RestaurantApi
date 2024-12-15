using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.Models
{
    public class OrderDetail
    {
        [Display(Name = "Order Id")]
        public int Order_Id { get; set; }

        [Display(Name = "Order Date")]
        public DateTime Order_Date { get; set; }

        [Display(Name = "Delivery Address")]
        public string Delivery_Address { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "User Phone")]
        public string User_Phone { get; set; }

        [Display(Name = "Dish")]
        public string Dish_Name { get; set; }

        public decimal Price { get; set; }
    }
}

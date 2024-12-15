using Microsoft.EntityFrameworkCore.Migrations;
using RestaurantApi.Utils;

#nullable disable

namespace RestaurantApi.Migrations
{
    public partial class InsertTestData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Role_Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "Restaurant_Id", "Restaurant_Name", "Restaurant_Address", "Restaurant_Phone", "Rating", "Category" },
                values: new object[,]
                {
                    { 1, "Pizza Place", "123 Main St", "1234567890", 8.5m, "Italian" },
                    { 2, "Sushi World", "456 Ocean Ave", "0987654321", 9.0m, "Japanese" },
                    { 3, "Burger House", "789 Burger Blvd", "1122334455", 7.8m, "American" },
                    { 4, "Taco Town", "987 Taco St", "4455667788", 8.2m, "Mexican" },
                    { 5, "Pasta Paradise", "321 Pasta Rd", "5566778899", 9.1m, "Italian" },
                    { 6, "BBQ Barn", "654 BBQ Ln", "6677889900", 8.4m, "Barbecue" },
                    { 7, "Salad Spot", "111 Veggie Way", "7788990011", 8.7m, "Healthy" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "User_Id", "First_Name", "Last_Name", "Username", "Password_Hash", "User_Address", "User_Phone", "Email", "Role_Id" },
                values: new object[,]
                {
                    { 1, "Admin", "User", "admin", PasswordHasher.HashPassword("admin123"), "Admin Lane", "1111111111", "admin@example.com", 1 },
                    { 2, "John", "Doe", "johndoe", PasswordHasher.HashPassword("password1"), "User Lane", "2222222222", "john@example.com", 2 },
                    { 3, "Jane", "Smith", "janesmith", PasswordHasher.HashPassword("password2"), "User St", "3333333333", "jane@example.com", 2 },
                    { 4, "Emily", "Clark", "emilyc", PasswordHasher.HashPassword("password3"), "Clark Rd", "4444444444", "emily@example.com", 2 },
                    { 5, "Michael", "Brown", "michaelb", PasswordHasher.HashPassword("password4"), "Brown St", "5555555555", "michael@example.com", 2 },
                    { 6, "Sarah", "Taylor", "saraht", PasswordHasher.HashPassword("password5"), "Taylor Ave", "6666666666", "sarah@example.com", 2 },
                    { 7, "David", "Wilson", "davidw", PasswordHasher.HashPassword("password6"), "Wilson Dr", "7777777777", "david@example.com", 2 }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Dish_Id", "Dish_Name", "Description", "Price", "Category", "Restaurant_Id" },
                values: new object[,]
                {
                    { 1, "Margherita Pizza", "Classic margherita with mozzarella and tomato", 10.99m, "Pizza", 1 },
                    { 2, "Pepperoni Pizza", "Spicy pepperoni with cheese", 12.99m, "Pizza", 1 },
                    { 3, "Sushi Roll", "Fresh salmon and avocado roll", 15.99m, "Sushi", 2 },
                    { 4, "Dragon Roll", "Spicy tuna with crispy topping", 18.99m, "Sushi", 2 },
                    { 5, "Cheeseburger", "Classic beef cheeseburger", 8.99m, "Burger", 3 },
                    { 6, "BBQ Burger", "Beef burger with BBQ sauce", 9.99m, "Burger", 3 },
                    { 7, "Taco", "Soft taco with beef and veggies", 6.99m, "Mexican", 4 },
                    { 8, "Quesadilla", "Cheese quesadilla with salsa", 7.99m, "Mexican", 4 },
                    { 9, "Pasta Alfredo", "Creamy Alfredo sauce with fettuccine", 12.99m, "Pasta", 5 },
                    { 10, "Pasta Carbonara", "Pasta with bacon and creamy sauce", 13.99m, "Pasta", 5 }
                });

            migrationBuilder.InsertData(
                    table: "Orders",
                    columns: new[] { "Order_Id", "User_Id", "Restaurant_Id", "Order_Date", "Delivery_Address", "Status" },
                    values: new object[,]
                    {
                        { 1, 2, 1, DateTime.Now.AddDays(-1), "User Lane 123", "Completed" },
                        { 2, 3, 2, DateTime.Now.AddDays(-2), "User St 456", "Pending" },
                        { 3, 4, 3, DateTime.Now.AddDays(-3), "Clark Rd 789", "Pending" },
                        { 4, 5, 4, DateTime.Now.AddDays(-4), "Brown St 101", "Completed" },
                        { 5, 6, 5, DateTime.Now.AddDays(-5), "Taylor Ave 202", "Pending" }
                    });

            migrationBuilder.InsertData(
                table: "OrderDishes",
                columns: new[] { "Order_Id", "Dish_Id", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 2 },
                    { 1, 2, 3 },
                    { 2, 3, 2 },
                    { 2, 4, 1 },
                    { 3, 5, 2 },
                    { 3, 6, 2 },
                    { 4, 7, 3 },
                    { 4, 8, 1 },
                    { 5, 9, 2 },
                    { 5, 10, 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM OrderDishes");
            migrationBuilder.Sql("DELETE FROM Orders");
            migrationBuilder.Sql("DELETE FROM Dishes");
            migrationBuilder.Sql("DELETE FROM Users");
            migrationBuilder.Sql("DELETE FROM Restaurants");
            migrationBuilder.Sql("DELETE FROM Roles");
        }
    }
}

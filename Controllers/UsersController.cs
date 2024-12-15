using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;
using RestaurantApi.DTOs;
using RestaurantApi.Models;
using RestaurantApi.Utils;

namespace RestaurantApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly FoodDeliveryContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(FoodDeliveryContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            _logger.LogInformation("Fetching users from the database.");

            var users = await _context
                .Users.Select(u => new UserDto
                {
                    User_Id = u.User_Id,
                    First_Name = u.First_Name,
                    Last_Name = u.Last_Name,
                    Username = u.Username,
                    User_Address = u.User_Address,
                    User_Phone = u.User_Phone,
                    Email = u.Email,
                    Role_Id = u.Role_Id,
                    Orders = u
                        .Orders.Select(o => new OrderDto
                        {
                            Order_Id = o.Order_Id,
                            User_Id = o.User_Id,
                            Restaurant_Id = o.Restaurant_Id,
                            Order_Date = o.Order_Date,
                            Delivery_Address = o.Delivery_Address
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context
                .Users.Select(u => new UserDto
                {
                    User_Id = u.User_Id,
                    First_Name = u.First_Name,
                    Last_Name = u.Last_Name,
                    Username = u.Username,
                    User_Address = u.User_Address,
                    User_Phone = u.User_Phone,
                    Email = u.Email,
                    Role_Id = u.Role_Id,
                    Orders = u
                        .Orders.Select(o => new OrderDto
                        {
                            Order_Id = o.Order_Id,
                            User_Id = o.User_Id,
                            Restaurant_Id = o.Restaurant_Id,
                            Order_Date = o.Order_Date,
                            Delivery_Address = o.Delivery_Address
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(u => u.User_Id == id);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new user.");

            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return BadRequest("Username already exists.");
            }

            if (string.IsNullOrWhiteSpace(userDto.Password))
            {
                return BadRequest("Password cannot be empty.");
            }

            var user = new User
            {
                First_Name = userDto.First_Name,
                Last_Name = userDto.Last_Name,
                Username = userDto.Username,
                User_Address = userDto.User_Address,
                User_Phone = userDto.User_Phone,
                Email = userDto.Email,
                Role_Id = userDto.Role_Id,
                Password_Hash = PasswordHasher.HashPassword(userDto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with ID {user.User_Id} created successfully.");
            return CreatedAtAction(nameof(GetUser), new { id = user.User_Id }, user);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return NotFound();
            }

            if (updatedUser.Username != existingUser.Username && await _context.Users.AnyAsync(u => u.Username == updatedUser.Username))
            {
                return BadRequest("Username already exists.");
            }

            var roleExists = await _context.Roles.AnyAsync(r => r.Role_Id == updatedUser.Role_Id);
            if (!roleExists)
            {
                _logger.LogWarning($"Role with ID {updatedUser.Role_Id} does not exist.");
                return BadRequest($"Role with ID {updatedUser.Role_Id} does not exist.");
            }

            existingUser.First_Name = updatedUser.First_Name ?? existingUser.First_Name;
            existingUser.Last_Name = updatedUser.Last_Name ?? existingUser.Last_Name;
            existingUser.Username = updatedUser.Username ?? existingUser.Username;
            existingUser.User_Address = updatedUser.User_Address ?? existingUser.User_Address;
            existingUser.User_Phone = updatedUser.User_Phone ?? existingUser.User_Phone;
            existingUser.Email = updatedUser.Email ?? existingUser.Email;
            existingUser.Role_Id = updatedUser.Role_Id;

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                existingUser.Password_Hash = PasswordHasher.HashPassword(updatedUser.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User with ID {id} updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.User_Id == id))
                {
                    _logger.LogWarning($"User with ID {id} not found for update.");
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPatch("{id}/password")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] string newPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return BadRequest("Password cannot be empty.");
            }

            existingUser.Password_Hash = PasswordHasher.HashPassword(newPassword);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Password for User ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating password for User ID {id}: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the password.");
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context
                .Users.Include(u => u.Orders)
                .ThenInclude(o => o.OrderDishes)
                .FirstOrDefaultAsync(u => u.User_Id == id);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found for deletion.");
                return NotFound();
            }

            foreach (var order in user.Orders)
            {
                _context.OrderDishes.RemoveRange(order.OrderDishes);
            }

            _context.Orders.RemoveRange(user.Orders);

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"User with ID {id} deleted successfully.");

            return NoContent();
        }
    }
}

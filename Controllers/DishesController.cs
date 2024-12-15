using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;
using RestaurantApi.DTOs;
using RestaurantApi.Models;

namespace RestaurantApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DishesController : ControllerBase
    {
        private readonly FoodDeliveryContext _context;
        private readonly ILogger<DishesController> _logger;

        public DishesController(FoodDeliveryContext context, ILogger<DishesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishDto>>> GetDishes()
        {
            var dishes = await _context.Dishes
                .Select(d => new DishDto
                {
                    Dish_Id = d.Dish_Id,
                    Dish_Name = d.Dish_Name,
                    Description = d.Description,
                    Price = d.Price,
                    Category = d.Category,
                    Restaurant_Id = d.Restaurant_Id
                })
                .ToListAsync();

            return Ok(dishes);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DishDto>> GetDish(int id)
        {
            var dish = await _context.Dishes
                .Select(d => new DishDto
                {
                    Dish_Id = d.Dish_Id,
                    Dish_Name = d.Dish_Name,
                    Description = d.Description,
                    Price = d.Price,
                    Category = d.Category,
                    Restaurant_Id = d.Restaurant_Id
                })
                .FirstOrDefaultAsync(d => d.Dish_Id == id);

            if (dish == null)
            {
                _logger.LogWarning($"Dish with ID {id} not found.");
                return NotFound();
            }

            return Ok(dish);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Dish>> CreateDish([FromBody] DishCreateDto dishDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Restaurant_Id == dishDto.Restaurant_Id);
            if (!restaurantExists)
            {
                return BadRequest($"Restaurant with ID {dishDto.Restaurant_Id} does not exist.");
            }

            var dish = new Dish
            {
                Dish_Name = dishDto.Dish_Name,
                Description = dishDto.Description,
                Price = dishDto.Price,
                Category = dishDto.Category,
                Restaurant_Id = dishDto.Restaurant_Id
            };

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Dish with ID {dish.Dish_Id} created successfully.");
            return CreatedAtAction(nameof(GetDish), new { id = dish.Dish_Id }, dish);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDish(int id, [FromBody] DishUpdateDto updatedDish)
        {
            if (id != updatedDish.Dish_Id)
            {
                _logger.LogWarning($"Dish ID mismatch: {id} != {updatedDish.Dish_Id}");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                _logger.LogWarning($"Dish with ID {id} not found.");
                return NotFound();
            }

            dish.Dish_Name = updatedDish.Dish_Name;
            dish.Description = updatedDish.Description;
            dish.Price = updatedDish.Price;
            dish.Category = updatedDish.Category;
            dish.Restaurant_Id = updatedDish.Restaurant_Id;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Dish with ID {id} updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Dishes.Any(d => d.Dish_Id == id))
                {
                    _logger.LogWarning($"Dish with ID {id} not found for update.");
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                _logger.LogWarning($"Dish with ID {id} not found for deletion.");
                return NotFound();
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Dish with ID {id} deleted successfully.");

            return NoContent();
        }
    }
}

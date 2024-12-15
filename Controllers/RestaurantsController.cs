using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RestaurantApi.Data;
using RestaurantApi.DTOs;
using RestaurantApi.Models;

namespace RestaurantApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly FoodDeliveryContext _context;
        private readonly ILogger<RestaurantsController> _logger;
        private readonly IMemoryCache _cache;

        public RestaurantsController(FoodDeliveryContext context, ILogger<RestaurantsController> logger, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRestaurants()
        {
            const string cacheKey = "GetRestaurantsCache";
            if (!_cache.TryGetValue(cacheKey, out List<RestaurantDto> restaurants))
            {
                _logger.LogInformation("Fetching restaurants from the database.");
                restaurants = await _context.Restaurants
                    .Select(r => new RestaurantDto
                    {
                        Restaurant_Id = r.Restaurant_Id,
                        Restaurant_Name = r.Restaurant_Name,
                        Restaurant_Address = r.Restaurant_Address,
                        Restaurant_Phone = r.Restaurant_Phone,
                        Rating = r.Rating,
                        Category = r.Category,
                        Dishes = r.Dishes.Select(d => new DishDto
                        {
                            Dish_Id = d.Dish_Id,
                            Dish_Name = d.Dish_Name,
                            Price = d.Price,
                            Description = d.Description,
                            Category = d.Category,
                            Restaurant_Id = d.Restaurant_Id
                        }).ToList()
                    })
                    .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, restaurants, cacheOptions);
            }
            else
            {
                _logger.LogInformation("Returning cached restaurants.");
            }

            return Ok(restaurants);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurants
                .Select(r => new RestaurantDto
                {
                    Restaurant_Id = r.Restaurant_Id,
                    Restaurant_Name = r.Restaurant_Name,
                    Restaurant_Address = r.Restaurant_Address,
                    Restaurant_Phone = r.Restaurant_Phone,
                    Rating = r.Rating,
                    Category = r.Category,
                    Dishes = r.Dishes.Select(d => new DishDto
                    {
                        Dish_Id = d.Dish_Id,
                        Dish_Name = d.Dish_Name,
                        Price = d.Price,
                        Description = d.Description,
                        Category = d.Category,
                        Restaurant_Id = d.Restaurant_Id
                    }).ToList()
                })
                .FirstOrDefaultAsync(r => r.Restaurant_Id == id);

            if (restaurant == null)
            {
                _logger.LogWarning($"Restaurant with ID {id} not found.");
                return NotFound();
            }

            return Ok(restaurant);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<Restaurant>> CreateRestaurant([FromBody] RestaurantCreateDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new restaurant.");

            var restaurant = new Restaurant
            {
                Restaurant_Name = restaurantDto.Restaurant_Name,
                Restaurant_Address = restaurantDto.Restaurant_Address,
                Restaurant_Phone = restaurantDto.Restaurant_Phone,
                Rating = restaurantDto.Rating,
                Category = restaurantDto.Category
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            _cache.Remove("GetRestaurantsCache");
            _logger.LogInformation($"Restaurant with ID {restaurant.Restaurant_Id} created successfully.");
            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Restaurant_Id }, restaurant);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto updatedRestaurant)
        {
            if (id != updatedRestaurant.Restaurant_Id)
            {
                _logger.LogWarning($"Restaurant ID mismatch: {id} != {updatedRestaurant.Restaurant_Id}");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                _logger.LogWarning($"Restaurant with ID {id} not found.");
                return NotFound();
            }

            restaurant.Restaurant_Name = updatedRestaurant.Restaurant_Name;
            restaurant.Restaurant_Address = updatedRestaurant.Restaurant_Address;
            restaurant.Restaurant_Phone = updatedRestaurant.Restaurant_Phone;
            restaurant.Rating = updatedRestaurant.Rating;
            restaurant.Category = updatedRestaurant.Category;

            try
            {
                await _context.SaveChangesAsync();
                _cache.Remove("GetRestaurantsCache");
                _logger.LogInformation($"Restaurant with ID {id} updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Restaurants.Any(r => r.Restaurant_Id == id))
                {
                    _logger.LogWarning($"Restaurant with ID {id} not found for update.");
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                _logger.LogWarning($"Restaurant with ID {id} not found for deletion.");
                return NotFound();
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
            _cache.Remove("GetRestaurantsCache");
            _logger.LogInformation($"Restaurant with ID {id} deleted successfully.");

            return NoContent();
        }
    }
}
    
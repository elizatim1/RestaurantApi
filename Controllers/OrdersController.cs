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
    public class OrdersController : ControllerBase
    {
        private readonly FoodDeliveryContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(FoodDeliveryContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDishes)
                .Select(o => new OrderDto
                {
                    Order_Id = o.Order_Id,
                    User_Id = o.User_Id,
                    Restaurant_Id = o.Restaurant_Id,
                    Order_Date = o.Order_Date,
                    Status = o.Status,      
                    Delivery_Address = o.Delivery_Address,
                    OrderDetails = o.OrderDishes.Select(od => new OrderDetailDto
                    {
                        Dish_Id = od.Dish_Id,
                        Quantity = od.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDishes)
                .Select(o => new OrderDto
                {
                    Order_Id = o.Order_Id,
                    User_Id = o.User_Id,
                    Restaurant_Id = o.Restaurant_Id,
                    Order_Date = o.Order_Date,
                    Status = o.Status,
                    Delivery_Address = o.Delivery_Address,
                    OrderDetails = o.OrderDishes.Select(od => new OrderDetailDto
                    {
                        Dish_Id = od.Dish_Id,
                        Quantity = od.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync(o => o.Order_Id == id);

            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found.");
                return NotFound();
            }

            return Ok(order);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = await _context.Users.AnyAsync(u => u.User_Id == orderDto.User_Id);
            if (!userExists)
            {
                return BadRequest($"User with ID {orderDto.User_Id} does not exist.");
            }

            var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Restaurant_Id == orderDto.Restaurant_Id);
            if (!restaurantExists)
            {
                return BadRequest($"Restaurant with ID {orderDto.Restaurant_Id} does not exist.");
            }

            var dishIds = orderDto.OrderDetails.Select(od => od.Dish_Id).ToList();
            var dishesExist = await _context.Dishes.Where(d => dishIds.Contains(d.Dish_Id)).Select(d => d.Dish_Id).ToListAsync();
            var missingDishes = dishIds.Except(dishesExist).ToList();

            if (missingDishes.Any())
            {
                return BadRequest($"Dishes with IDs {string.Join(", ", missingDishes)} do not exist.");
            }

            var order = new Order
            {
                User_Id = orderDto.User_Id,
                Status = orderDto.Status,
                Restaurant_Id = orderDto.Restaurant_Id,
                Order_Date = orderDto.Order_Date,
                Delivery_Address = orderDto.Delivery_Address
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var detail in orderDto.OrderDetails)
            {
                var orderDish = new OrderDish
                {
                    Order_Id = order.Order_Id,
                    Dish_Id = detail.Dish_Id,
                    Quantity = detail.Quantity
                };
                _context.OrderDishes.Add(orderDish);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Order with ID {order.Order_Id} created successfully.");
            return CreatedAtAction(nameof(GetOrder), new { id = order.Order_Id }, order);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDto updatedOrder)
        {
            if (id != updatedOrder.Order_Id)
            {
                _logger.LogWarning($"Order ID mismatch: {id} != {updatedOrder.Order_Id}");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.Include(o => o.OrderDishes).FirstOrDefaultAsync(o => o.Order_Id == id);
            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found.");
                return NotFound();
            }

            order.User_Id = updatedOrder.User_Id;
            order.Status = updatedOrder.Status;
            order.Restaurant_Id = updatedOrder.Restaurant_Id;
            order.Order_Date = updatedOrder.Order_Date;
            order.Delivery_Address = updatedOrder.Delivery_Address;

            _context.OrderDishes.RemoveRange(order.OrderDishes);

            foreach (var detail in updatedOrder.OrderDetails)
            {
                var orderDish = new OrderDish
                {
                    Order_Id = order.Order_Id,
                    Dish_Id = detail.Dish_Id,
                    Quantity = detail.Quantity
                };
                _context.OrderDishes.Add(orderDish);
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order with ID {id} updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(o => o.Order_Id == id))
                {
                    _logger.LogWarning($"Order with ID {id} not found for update.");
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)    
        {
            var order = await _context.Orders.Include(o => o.OrderDishes).FirstOrDefaultAsync(o => o.Order_Id == id);
            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found for deletion.");
                return NotFound();
            }

            _context.OrderDishes.RemoveRange(order.OrderDishes);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Order with ID {id} deleted successfully.");

            return NoContent();
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetOrderStats()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");

            return Ok(new
            {
                TotalOrders = totalOrders,
                CompletedOrders = completedOrders,
                PendingOrders = pendingOrders
            });
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using RestaurantApi.Models;
using RestaurantApi.Data;

namespace RestaurantApi.Services
{
    public class OrderEvents
    {
        private readonly FoodDeliveryContext _context;

        public OrderEvents(FoodDeliveryContext context)
        {
            _context = context;
        }

        public event EventHandler<OrderEventArgs> OrderAdded;
        public event EventHandler<OrderEventArgs> OrderUpdated;

        public void OnOrderAdded(OrderEventArgs args)
        {
            if (args.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only Admins can create orders.");
            }

            foreach (var orderDish in args.OrderDishes)
            {
                if (!_context.Dishes.Any(d => d.Dish_Id == orderDish.Dish_Id))
                {
                    throw new ArgumentException($"Dish with ID {orderDish.Dish_Id} does not exist.");
                }

                _context.OrderDishes.Add(new OrderDish
                {
                    Order_Id = args.OrderId,
                    Dish_Id = orderDish.Dish_Id,
                    Quantity = orderDish.Quantity
                });
            }

            _context.SaveChanges();

            OrderAdded?.Invoke(this, args);
        }

        public void OnOrderUpdated(OrderEventArgs args)
        {
            if (args.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only Admins can update orders.");
            }

            var existingOrderDishes = _context.OrderDishes.Where(od => od.Order_Id == args.OrderId).ToList();
            _context.OrderDishes.RemoveRange(existingOrderDishes);

            foreach (var orderDish in args.OrderDishes)
            {
                if (!_context.Dishes.Any(d => d.Dish_Id == orderDish.Dish_Id))
                {
                    throw new ArgumentException($"Dish with ID {orderDish.Dish_Id} does not exist.");
                }

                _context.OrderDishes.Add(new OrderDish
                {
                    Order_Id = args.OrderId,
                    Dish_Id = orderDish.Dish_Id,
                    Quantity = orderDish.Quantity
                });
            }

            _context.SaveChanges();

            OrderUpdated?.Invoke(this, args);
        }
    }

    public class OrderEventArgs : EventArgs
    {
        public int OrderId { get; set; }
        public string Role { get; set; }
        public List<OrderDish> OrderDishes { get; set; }
        public string Action { get; set; }
    }
}

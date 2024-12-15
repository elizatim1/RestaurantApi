using System;

namespace RestaurantApi.Services
{
    public class DishEvents
    {
        public event EventHandler<DishEventArgs> DishAdded;
        public event EventHandler<DishEventArgs> DishUpdated;

        public void OnDishAdded(DishEventArgs args)
        {
            DishAdded?.Invoke(this, args);
        }

        public void OnDishUpdated(DishEventArgs args)
        {
            DishUpdated?.Invoke(this, args);
        }
    }

    public class DishEventArgs : EventArgs
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public string Action { get; set; }
    }
}

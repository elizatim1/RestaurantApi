using System;

namespace RestaurantApi.Services
{
    public class UserEvents
    {
        public event EventHandler<UserEventArgs> UserAdded;
        public event EventHandler<UserEventArgs> UserUpdated;

        public void OnUserAdded(UserEventArgs args)
        {
            if (args.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only Admins can create new users.");
            }

            UserAdded?.Invoke(this, args);
        }

        public void OnUserUpdated(UserEventArgs args)
        {
            if (args.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Only Admins can update user records.");
            }

            UserUpdated?.Invoke(this, args);
        }
    }

    public class UserEventArgs : EventArgs
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Action { get; set; }
    }
}

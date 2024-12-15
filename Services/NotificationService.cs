namespace RestaurantApi.Services
{
    public class NotificationService
    {
        public string SuccessMessage { get; private set; }
        public string ErrorMessage { get; private set; }

        public void NotifySuccess(string message)
        {
            SuccessMessage = message;
        }

        public void NotifyError(string message, string role)
        {
            if (role != "Admin")
            {
                ErrorMessage = "Only Admins can perform this action.";
            }
            else
            {
                ErrorMessage = message;
            }
        }
    }
}

namespace CoffeeShop.Enumerations
{
    public static class UserRoles
    {
        public const string Admin = "Administrator";
        public const string Owner = "Owner";
        public const string User = "User";

        public static string[] GetRoles()
        {
            return new string[]
            {
                Admin,
                Owner,
                User
            };
        }
    }
}
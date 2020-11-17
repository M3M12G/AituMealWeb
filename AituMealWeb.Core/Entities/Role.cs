namespace AituMealWeb.Core.Entities
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string Kassir = "Kassir";
        public const string User = "User";
        public const string AdminOrUser = Admin + "," + User;
        public const string AdminOrKassir = Admin + "," + Kassir;
    }
}

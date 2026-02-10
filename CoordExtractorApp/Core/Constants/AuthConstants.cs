namespace CoordExtractorApp.Core.Constants
{
    public static class AuthConstants
    {
        public const string AdminRole = "Admin";
        public const string ManagerRole = "Manager";
        public const string MemberRole = "Member";

        // Combined roles for authorization attributes
        public const string AdminOrManager = "Admin, Manager";
    }
}

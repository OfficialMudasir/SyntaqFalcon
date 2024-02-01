namespace Syntaq.Falcon.Authorization.Roles
{
    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";
        }

        public static class Tenants
        {
            public const string Admin = "Admin";

            public const string User = "User";

            //STQ MODIFIED
            public const string Author = "Author";
            public const string Builder = "Builder";
            //STQ MODIFIED

        }
    }
}
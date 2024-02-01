namespace Syntaq.Falcon.Authorization.Users
{
    public class UserConsts
    {
        public const int MaxPhoneNumberLength = 24;

        // STQ MODIFIED
        public const int MaxABNNumberLength = 11;
        public const int MinABNNumberLength = 11;
        public const string ABNRegex = "^[0-9]+$";
    }
}

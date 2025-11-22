namespace LastManagement.Api.Global.Constants;

public static class CustomerConstants
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;

    public static class Status
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Suspended = "Suspended";
    }
}
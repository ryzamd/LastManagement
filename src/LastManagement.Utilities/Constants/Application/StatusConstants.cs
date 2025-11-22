namespace LastManagement.Application.Constants;

public static class StatusConstants
{
    public static class BatchOperation
    {
        public const string SUCCESS = "success";
        public const string ERROR = "error";
    }

    public static class LastSize
    {
        public const string ACTIVE = "Active";
        public const string DISCONTINUED = "Discontinued";
        public const string REPLACED = "Replaced";
    }

    public static class LastName
    {
        public const string ACTIVE = "Active";
        public const string DISCONTINUED = "Discontinued";
        public const string REPLACED = "Replaced";
    }

    public static class Customer
    {
        public const string ACTIVE = "Active";
        public const string INACTIVE = "Inactive";
        public const string SUSPENDED = "Suspended";
    }

    public static class LocationType
    {
        public const string PRODUCTION = "Production";
        public const string DEVELOPMENT = "Development";
        public const string QUALITY = "Quality";
        public const string STORAGE = "Storage";
    }
}

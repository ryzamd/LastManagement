namespace LastManagement.Api.Constants
{
    public static class RegexPattern
    {
        public static class Customer
        {
            public const string GET_CUSTOMERS = @"status\s+eq\s+'(\w+)'";
        }

        public static class Location
        {
            public const string LOCATION_TYPE_FILTER = @"locationType\s+eq\s+'(\w+)'";
            public const string IS_ACTIVE_FILTER = @"isActive\s+eq\s+(true|false)";
        }

        public static class LastSize
        {
            public const string STATUS_ACTIVE_FILTER = "status eq 'active'";
            public const string STATUS_DISCONTINUED_FILTER = "status eq 'discontinued'";
            public const string STATUS_REPLACED_FILTER = "status eq 'replaced'";
        }
    }
}

namespace LastManagement.Api.Constants;

public static class ConfigurationKeys
{
    public const string DATABASE = "Database";
    public const string JWT = "Jwt";

    public static class Database
    {
        public const string SECTION = "Database";
        public const string DEFAULT_CONNECTION = "DefaultConnection";
    }

    public static class Cors
    {
        public const string ALLOWED_ORIGINS = "Cors:AllowedOrigins";
    }

    public static class Logging
    {
        public const string SECTION = "Logging";
    }
}

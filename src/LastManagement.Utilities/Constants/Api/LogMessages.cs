namespace LastManagement.Api.Constants;

public static class LogMessages
{
    public static class Application
    {
        public const string STARTING_SERVER = "Starting Last Management Server";
        public const string TERMINATED_UNEXPECTEDLY = "Application terminated unexpectedly";
    }

    public static class Exceptions
    {
        public const string UNHANDLED_EXCEPTION = "An unhandled exception occurred";
    }

    public static class Configuration
    {
        public const string JWT_MISSING = "JWT configuration is missing";
    }
}

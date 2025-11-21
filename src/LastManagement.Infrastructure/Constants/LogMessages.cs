namespace LastManagement.Infrastructure.Constants;

/// <summary>
/// Contains log message templates for the Infrastructure layer
/// </summary>
public static class LogMessages
{
    /// <summary>
    /// Idempotency cleanup service messages
    /// </summary>
    public static class IdempotencyCleanup
    {
        public const string STARTED = "IdempotencyCleanupService started";
        public const string STOPPED = "IdempotencyCleanupService stopped";
        public const string CLEANED_UP = "Expired idempotency keys cleaned up at {Time}";
        public const string ERROR = "Error cleaning up expired idempotency keys";
    }
    
    /// <summary>
    /// Database-related log messages
    /// </summary>
    public static class Database
    {
        public const string VIEW_NOT_FOUND = "Database view {0} not found. Run migrations.";
        public const string MIGRATION_APPLIED = "Migration {0} applied successfully";
        public const string CONNECTION_OPENED = "Database connection opened";
        public const string CONNECTION_CLOSED = "Database connection closed";
    }
}

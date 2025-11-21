namespace LastManagement.Infrastructure.Constants;

/// <summary>
/// Contains repository-related constants like sort fields and query patterns
/// </summary>
public static class RepositoryConstants
{
    /// <summary>
    /// Sort field names used in repositories
    /// </summary>
    public static class SortFields
    {
        // Model code sorting
        public const string MODEL_CODE = "modelcode";
        public const string MODEL_CODE_ASC = "modelcode asc";
        public const string MODEL_CODE_DESC = "modelcode desc";
        
        // Created at sorting
        public const string CREATED_AT = "createdat";
        public const string CREATED_AT_ASC = "createdat asc";
        public const string CREATED_AT_DESC = "createdat desc";
        
        // Customer name sorting
        public const string CUSTOMER_NAME = "customername";
        public const string CUSTOMER_NAME_ASC = "customername asc";
        public const string CUSTOMER_NAME_DESC = "customername desc";
    }
    
    /// <summary>
    /// SQL query patterns and snippets
    /// </summary>
    public static class SqlPatterns
    {
        public const string WHERE_CLAUSE_START = "WHERE 1=1";
        public const string ORDER_BY = "ORDER BY";
        public const string SELECT = "SELECT";
        public const string FROM = "FROM";
    }
    
    /// <summary>
    /// Information schema queries
    /// </summary>
    public static class InformationSchema
    {
        public const string CHECK_VIEW_EXISTS = "SELECT 1 FROM information_schema.views WHERE table_name = {0} AND table_schema = 'public'";
    }
}

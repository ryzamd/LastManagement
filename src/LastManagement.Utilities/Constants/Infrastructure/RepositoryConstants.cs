namespace LastManagement.Infrastructure.Constants;

public static class RepositoryConstants
{
    public static class SortFields
    {
        public const string MODEL_CODE = "modelcode";
        public const string MODEL_CODE_ASC = "modelcode asc";
        public const string MODEL_CODE_DESC = "modelcode desc";

        public const string CREATED_AT = "createdat";
        public const string CREATED_AT_ASC = "createdat asc";
        public const string CREATED_AT_DESC = "createdat desc";

        public const string CUSTOMER_NAME = "customername";
        public const string CUSTOMER_NAME_ASC = "customername asc";
        public const string CUSTOMER_NAME_DESC = "customername desc";
    }

    public static class SqlPatterns
    {
        public const string WHERE_CLAUSE_START = "WHERE 1=1";
        public const string ORDER_BY = "ORDER BY";
        public const string SELECT = "SELECT";
        public const string FROM = "FROM";
    }

    public static class InformationSchema
    {
        public const string CHECK_VIEW_EXISTS = "SELECT 1 FROM information_schema.views WHERE table_name = {0} AND table_schema = 'public'";
    }
}

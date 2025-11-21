namespace LastManagement.Application.Constants;

public static class FormatConstants
{
    public static class DateTime
    {
        public const string DATE_ONLY_FORMAT = "yyyyMMdd";
        public const string ISO_8601 = "yyyy-MM-ddTHH:mm:ssZ";
    }

    public static class PurchaseOrder
    {
        public const string ORDER_NUMBER_PREFIX = "PO-";
        public const string ORDER_NUMBER_TEMPLATE = "PO-{0}-{1:D5}";
        public const int SEQUENCE_WIDTH = 5;
        public const int MAX_SEQUENCE_PER_DAY = 99999;
    }

    public static class RegexPatterns
    {
        public const string LOCATION_TYPE = "^(Production|Development|Quality|Storage)$";
        public const string LAST_SIZE_STATUS = "^(Active|Discontinued|Replaced)$";
        public const string LAST_NAME_STATUS = "^(Active|Discontinued|Replaced)$";
        public const string CUSTOMER_STATUS = "^(Active|Inactive|Suspended)$";
    }
}

namespace LastManagement.Utilities.Constants.Global;

public static class ResultMessages
{
    public static class Authentication
    {
        public const string INVALID_CREDENTIALS = "Invalid username or password";
        public const string INVALID_REFRESH_TOKEN = "Invalid or expired refresh token";
        public const string REFRESH_TOKEN_EXPIRED = "Refresh token has expired";
        public const string ACCOUNT_NOT_FOUND = "Account not found";
        public const string USER_NOT_FOUND = "User not found";
        public const string JWT_SECRET_KEY_MISSING = "JWT SecretKey is null or empty";
    }

    public static class Customer
    {
        public const string NOT_FOUND = "Customer not found";
        public const string MODIFIED_BY_ANOTHER = "Customer was modified by another user";
        public const string INVALID_STATUS = "Invalid status value";
        public const string CANNOT_DELETE_HAS_LASTS = "Cannot delete customer because it has associated lasts";
        public const string CUSTOMER_ALREADY_EXISTS = "Customer with name {0} already exists";
    }

    public static class Location
    {
        public const string NOT_FOUND = "Location not found";
        public const string INVALID_TYPE = "Invalid location type";
        public const string CANNOT_DELETE_HAS_INVENTORY = "Cannot delete location because it has inventory";
    }

    public static class LastSize
    {
        public const string CANNOT_DELETE_HAS_INVENTORY = "Cannot delete size because it is used in inventory";
    }

    public static class InventoryStock
    {
        public const string SAME_SOURCE_DESTINATION = "Source and destination locations cannot be the same";
    }

    public static class Common
    {
        public const string SUCCESS_FAILURE_ERROR_MISMATCH = "Success result cannot have error";
        public const string FAILURE_REQUIRES_ERROR = "Failure result must have error";
    }
}
namespace LastManagement.Application.Constants;

public static class ErrorMessages
{
    public static class PurchaseOrder
    {
        public const string NOT_FOUND = "Purchase order with ID {0} not found";
        public const string MAX_ORDERS_EXCEEDED = "Maximum orders per day (99999) exceeded";
        public const string INVALID_STATUS = "Invalid status value: {0}";
    }

    public static class Location
    {
        public const string NOT_FOUND = "Location not found";
        public const string ALREADY_EXISTS = "Location with code '{0}' already exists";
        public const string CANNOT_DELETE_HAS_INVENTORY = "Cannot delete location because it has inventory";
        public const string INVALID_TYPE = "Invalid location type";
        public const string DOES_NOT_EXIST = "Location with ID {0} does not exist";
    }

    public static class LastSize
    {
        public const string NOT_FOUND = "Last size with ID {0} not found";
        public const string ALREADY_EXISTS = "Last size with value {0} and label '{1}' already exists";

        public const string INVALID_STATUS = "Invalid status value";
        public const string VALIDATION_ERROR = "One or more validation errors occurred";
        public const string CREATE_LAST_SIZE_ERROR = "An error occurred while creating the last size";
        public const string CREATE_BATCH_LAST_SIZE_ERROR = "An error occurred while creating batch sizes";
        public const string UPDATE_LAST_SIZE_ERROR = "An error occurred while updating the last size";
        public const string UPDATE_BATCH_LAST_SIZE_ERROR = "An error occurred while updating batch sizes";
        public const string DELETE_LAST_SIZE_EROR = "An error occurred while deleting the last size";
        public const string DELETE_BATCH_LAST_SIZE_ERROR = "An error occurred while deleting batch sizes";
        public const string DELETE_lAST_SIZE_IN_USE_ERROR = "Cannot delete size because it is used in inventory";
        public const string RETREVING_LAST_SIZE_ERROR = "An error occurred while retrieving last sizes";

        public const string SIZE_IS_IN_USE = "Size is in use in inventory";
        public const string SIZE_IS_IN_USE_CANNOT_UPDATE = "Cannot update size that is used in inventory";
    }

    public static class LastName
    {
        public const string NOT_FOUND = "Last name with ID {0} not found";
        public const string DOES_NOT_EXIST = "Last with ID {0} does not exist";
        public const string ALREADY_EXISTS = "Last code '{0}' already exists for customer {1}";
        public const string INVALID_STATUS = "Invalid status value: {0}";

        public const string RESOURCE_WAS_MODIFIED_BY_ANOTHER_REQUEST = "The resource was modified by another request. Please refresh and retry.";
        public const string CANNOT_DELETE_LAST_NAME_HAS_ASSOCIATED_RECORDS = "Cannot delete last name because it has associated records";
        public const string DELETE_LAST_NAME_ERROR = "An error occurred while deleting the last name";
        public const string BATCH_UPDATE_LAST_NAME_ERROR = "An error occurred while processing the batch update";
        public const string ID_MUST_BE_POSITIVE = "ID must be positive";
    }

    public static class LastModel
    {
        public const string NOT_FOUND = "Last model with ID {0} not found";
        public const string ALREADY_EXISTS = "Model code '{0}' already exists for last {1}";
        public const string DETAIL_ERROR = "An error occurred while retrieving models for last ID {0}";

        public const string POSITIVE_INTEGER_ERROR = "Must be a positive integer";
        public const string LAST_ID_MUST_BE_POSITIVE_INTERGER = "Last ID must be a positive integer";
    }

    public static class Customer
    {
        public const string NOT_FOUND = "Customer with ID {0} not found";
        public const string ALREADY_EXISTS = "Customer with name '{0}' already exists";

        public const string INVALID_STATUS = "Invalid status value";
    }

    public static class Size
    {
        public const string DOES_NOT_EXIST = "Size with ID {0} does not exist";
    }

    public static class Inventory
    {
        public const string INSUFFICIENT_STOCK = "Insufficient stock. Available: {0}, Requested: {1}";
        public const string CANNOT_DAMAGE = "Cannot damage {0} units. Only {1} good units available.";
        public const string CANNOT_REPAIR = "Cannot repair {0} units. Only {1} damaged units available.";
        public const string UNKNOWN_ADJUSTMENT_TYPE = "Unknown adjustment type: {0}";
        public const string CANNOT_RESERVE = "Cannot reserve {0} units. Only {1} available.";
        public const string CANNOT_RELEASE = "Cannot release {0} units. Only {1} reserved.";

        public const string THRESHOLD_MUST_BE_POSITIVE_INTEGER = "Threshold must be a positive integer";
    }
    public static class Stock
    {
        public const string NOT_FOUND = "Stock with ID {0} not found";
        public const string SOURCE_NOT_FOUND = "Source stock not found (Last: {0}, Size: {1}, Location: {2})";
        public const string SOURCE_LOCATION_NOT_FOUND = "Source location {0} not found";
        public const string DESTINATION_LOCATION_NOT_FOUND = "Destination location {0} not found";
        public const string INSUFFICIENT_AT_SOURCE = "Insufficient stock at source. Available: {0}, Requested: {1}";

        public const string IF_MATCH_REQUIRED_FOR_ADJUSTMENT = "If-Match header is required for stock adjustments";
        public const string STOCK_WAS_MODIFIED_BY_ANOTHER = "Stock was modified by another user";
    }

    public static class Account
    {
        public const string ACCOUT_NOT_FOUND = "Account not found";
        public const string USER_NOT_FOUND = "User not found";
        public const string CUSTOMER_NOT_FOUND = "Customer not found";
        public const string CANNOT_DELETE_CUSTOMER_HAS_LASTS = "Cannot delete customer because it has associated lasts";
    }
}

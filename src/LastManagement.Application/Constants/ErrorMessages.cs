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
    }

    public static class LastName
    {
        public const string NOT_FOUND = "Last name with ID {0} not found";
        public const string DOES_NOT_EXIST = "Last with ID {0} does not exist";
        public const string ALREADY_EXISTS = "Last code '{0}' already exists for customer {1}";
    }

    public static class LastModel
    {
        public const string NOT_FOUND = "Last model with ID {0} not found";
        public const string ALREADY_EXISTS = "Model code '{0}' already exists for last {1}";
    }

    public static class Customer
    {
        public const string NOT_FOUND = "Customer with ID {0} not found";
        public const string ALREADY_EXISTS = "Customer with name '{0}' already exists";
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
    }
}

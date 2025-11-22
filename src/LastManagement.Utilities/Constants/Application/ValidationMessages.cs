namespace LastManagement.Application.Constants;

public static class ValidationMessages
{
    public static class PurchaseOrder
    {
        public const string LOCATION_ID_REQUIRED = "Location ID must be positive";
        public const string REQUESTED_BY_REQUIRED = "Requested by must be between 1 and 200 characters";
        public const string DEPARTMENT_MAX_LENGTH = "Department must not exceed 200 characters";
        public const string NOTES_MAX_LENGTH = "Notes must not exceed 1000 characters";
        public const string ADMIN_NOTES_MAX_LENGTH = "Admin notes must not exceed 1000 characters";
        public const string AT_LEAST_ONE_ITEM = "At least one item is required";
    }

    public static class PurchaseOrderItem
    {
        public const string LAST_ID_REQUIRED = "Last ID must be positive";
        public const string SIZE_ID_REQUIRED = "Size ID must be positive";
        public const string QUANTITY_RANGE = "Quantity must be between 1 and 999999";
    }

    public static class Location
    {
        public const string CODE_REQUIRED = "Location code is required";
        public const string CODE_LENGTH = "Location code must be between 1 and 20 characters";
        public const string NAME_REQUIRED = "Location name is required";
        public const string NAME_LENGTH = "Location name must be between 1 and 100 characters";
        public const string TYPE_REQUIRED = "Location type is required";
        public const string TYPE_PATTERN = "^(Production|Development|Quality|Storage)$";
        public const string TYPE_INVALID = "Location type must be Production, Development, Quality, or Storage";
    }

    public static class LastSize
    {
        public const string SIZE_VALUE_REQUIRED = "Size value is required";
        public const string SIZE_VALUE_RANGE = "Size value must be between 0.1 and 99.9";
        public const string SIZE_LABEL_REQUIRED = "Size label is required";
        public const string SIZE_LABEL_LENGTH = "Size label must be between 1 and 20 characters";
        public const string STATUS_PATTERN = "^(Active|Discontinued|Replaced)$";
        public const string STATUS_INVALID = "Status must be Active, Discontinued, or Replaced";
        public const string AT_LEAST_ONE_SIZE = "At least one size is required";
        public const string AT_LEAST_ONE_OPERATION = "At least one operation is required";
        public const string AT_LEAST_ONE_ID = "At least one ID is required";
    }

    public static class LastName
    {
        public const string CUSTOMER_ID_REQUIRED = "Customer ID is required";
        public const string CUSTOMER_ID_POSITIVE = "Customer ID must be positive";
        public const string LAST_CODE_REQUIRED = "Last code is required";
        public const string LAST_CODE_LENGTH = "Last code must be between 1 and 50 characters";
        public const string STATUS_INVALID = "Status must be Active, Discontinued, or Replaced";
        public const string DISCONTINUE_REASON_MAX_LENGTH = "Discontinue reason cannot exceed 500 characters";
        public const string AT_LEAST_ONE_OPERATION = "At least one operation is required";
    }

    public static class LastModel
    {
        public const string LAST_ID_REQUIRED = "Last ID must be positive";
        public const string MODEL_CODE_REQUIRED = "Model code is required";
        public const string MODEL_CODE_LENGTH = "Model code must be between 1 and 50 characters";
    }

    public static class Customer
    {
        public const string CUSTOMER_NAME_REQUIRED = "Customer name is required";
        public const string CUSTOMER_NAME_LENGTH = "Customer name must be between 1 and 200 characters";
        public const string NAME_REQUIRED = "Customer name is required";
        public const string NAME_LENGTH = "Customer name must be between 1 and 200 characters";
        public const string STATUS_REQUIRED = "Status is required";
        public const string STATUS_INVALID = "Status must be Active, Inactive, or Suspended";
    }

    public static class Inventory
    {
        public const string QUANTITY_MIN = "Quantity must be at least 1";
        public const string QUANTITY_NON_NEGATIVE = "Quantity cannot be negative";
        public const string ADJUSTMENT_TYPE_REQUIRED = "Adjustment type is required";
        public const string MOVEMENT_TYPE_REQUIRED = "Movement type is required";
    }

    public static class Authentication
    {
        public const string USERNAME_REQUIRED = "Username is required";
        public const string USERNAME_LENGTH = "Username must be between 3 and 50 characters";
        public const string PASSWORD_REQUIRED = "Password is required";
        public const string PASSWORD_MIN_LENGTH = "Password must be at least 6 characters";
        public const string REFRESH_TOKEN_REQUIRED = "Refresh token is required";
    }

    public static class Common
    {
        public const string POSITIVE_INTEGER_REQUIRED = "Must be a positive integer";
        public const string ID_MUST_BE_POSITIVE = "{0} ID must be a positive integer";
    }
}

namespace LastManagement.Domain.Constants;

public static class DomainValidationMessages
{
    public static class Account
    {
        public const string USERNAME_EMPTY = "Username cannot be empty";
        public const string PASSWORD_HASH_EMPTY = "Password hash cannot be empty";
        public const string FULL_NAME_EMPTY = "Full name cannot be empty";
    }

    public static class Customer
    {
        public const string NAME_EMPTY = "Customer name cannot be empty";
        public const string NAME_EXCEEDS_LENGTH = "Customer name cannot exceed 200 characters";
    }

    public static class PurchaseOrder
    {
        public const string CANNOT_CONFIRM_NON_PENDING = "Cannot confirm order with status {0}. Order must be Pending.";
        public const string CANNOT_DENY_NON_PENDING = "Cannot deny order with status {0}. Order must be Pending.";

        public const string ORDER_NUMBER_EMPTY = "Order number cannot be empty";
        public const string REQUESTED_BY_EMPTY = "Requested by cannot be empty";
        public const string LOCATION_ID_POSITIVE = "Location ID must be positive";
        public const string REVIEWED_BY_EMPTY = "Reviewed by cannot be empty";
    }

    public static class PurchaseOrderItem
    {
        public const string ORDER_ID_POSITIVE = "Order ID must be positive";
        public const string LAST_ID_POSITIVE = "Last ID must be positive";
        public const string SIZE_ID_POSITIVE = "Size ID must be positive";
        public const string QUANTITY_POSITIVE = "Quantity must be positive";
    }

    public static class IdempotencyKey
    {
        public const string ORDER_ID_POSITIVE = "Order ID must be positive";
        public const string LAST_ID_POSITIVE = "Last ID must be positive";
        public const string SIZE_ID_POSITIVE = "Size ID must be positive";
        public const string QUANTITY_POSITIVE = "Quantity must be positive";
        public const string KEY_EMPTY = "Key cannot be empty";
        public const string RESULT_EMPTY = "Result cannot be empty";
        public const string EXPIRATION_FUTURE = "Expiration must be in the future";
    }

    public static class Location
    {
        public const string CODE_EMPTY = "Location code cannot be empty";
        public const string CODE_EXCEEDS_LENGTH = "Location code cannot exceed 20 characters";
        public const string NAME_EMPTY = "Location name cannot be empty";
        public const string NAME_EXCEEDS_LENGTH = "Location name cannot exceed 100 characters";
    }

    public static class LastSize
    {
        public const string VALUE_GREATER_THAN_ZERO = "Size value must be greater than zero";
        public const string LABEL_REQUIRED = "Size label is required";
        public const string LABEL_EXCEEDS_LENGTH = "Size label cannot exceed 20 characters";
        public const string ALREADY_DISCONTINUED = "Size is already discontinued";
        public const string ALREADY_ACTIVE = "Size is already active";
    }

    public static class LastName
    {
        public const string CUSTOMER_ID_POSITIVE = "Customer ID must be positive";
        public const string CODE_EMPTY = "Last code cannot be empty";
        public const string CODE_EXCEEDS_LENGTH = "Last code cannot exceed 50 characters";
        public const string DISCONTINUE_REASON_REQUIRED = "Discontinue reason is required when status is not Active";
        public const string CANNOT_TRANSFER_SAME_CUSTOMER = "Cannot transfer to the same customer";
    }

    public static class LastModel
    {
        public const string LAST_ID_POSITIVE = "Last ID must be positive";
        public const string CODE_REQUIRED = "Model code is required";
        public const string CODE_EMPTY = "Model code cannot be empty";
    }

    public static class InventoryStock
    {
        public const string INSUFFICIENT_STOCK = "Insufficient stock. Available: {0}, Requested: {1}";
        public const string CANNOT_DAMAGE_UNITS = "Cannot damage {0} units. Only {1} good units available.";
        public const string CANNOT_REPAIR_UNITS = "Cannot repair {0} units. Only {1} damaged units available.";
        public const string UNKNOWN_ADJUSTMENT_TYPE = "Unknown adjustment type: {0}";
        public const string RESERVE_QUANTITY_MUST_BE_POSITIVE = "Reserve quantity must be positive";
        public const string CANNOT_RESERVE_UNITS = "Cannot reserve {0} units. Only {1} available.";
        public const string RELEASE_QUANTITY_MUST_BE_POSITIVE = "Release quantity must be positive";
        public const string CANNOT_RELEASE_UNITS = "Cannot release {0} units. Only {1} reserved.";

        public const string INITIAL_QUANTITY_NEGATIVE = "Initial quantity cannot be negative";
        public const string QUANTITY_POSITIVE = "Quantity must be positive";
        public const string ADJUSTMENT_QUANTITY_POSITIVE = "Adjustment quantity must be positive";
        public const string RESERVE_QUANTITY_POSITIVE = "Reserve quantity must be positive";
        public const string RELEASE_QUANTITY_POSITIVE = "Release quantity must be positive";
    }

    public static class InventoryMovement
    {
        public const string QUANTITY_MUST_BE_POSITIVE = "Quantity must be positive";
    }
}

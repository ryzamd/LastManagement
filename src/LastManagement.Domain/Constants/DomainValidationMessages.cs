namespace LastManagement.Domain.Constants;

/// <summary>
/// Contains domain-level validation and business rule messages organized by entity
/// </summary>
public static class DomainValidationMessages
{
    /// <summary>
    /// Purchase Order validation messages
    /// </summary>
    public static class PurchaseOrder
    {
        public const string ORDER_NUMBER_EMPTY = "Order number cannot be empty";
        public const string REQUESTED_BY_EMPTY = "Requested by cannot be empty";
        public const string LOCATION_ID_MUST_BE_POSITIVE = "Location ID must be positive";
        public const string CANNOT_CONFIRM_NON_PENDING = "Cannot confirm order with status {0}. Order must be Pending.";
        public const string CANNOT_DENY_NON_PENDING = "Cannot deny order with status {0}. Order must be Pending.";
        public const string REVIEWED_BY_EMPTY = "Reviewed by cannot be empty";
    }
    
    /// <summary>
    /// Purchase Order Item validation messages
    /// </summary>
    public static class PurchaseOrderItem
    {
        public const string ORDER_ID_MUST_BE_POSITIVE = "Order ID must be positive";
        public const string LAST_ID_MUST_BE_POSITIVE = "Last ID must be positive";
        public const string SIZE_ID_MUST_BE_POSITIVE = "Size ID must be positive";
        public const string QUANTITY_MUST_BE_POSITIVE = "Quantity must be positive";
    }
    
    /// <summary>
    /// Idempotency Key validation messages
    /// </summary>
    public static class IdempotencyKey
    {
        public const string KEY_CANNOT_BE_EMPTY = "Key cannot be empty";
        public const string RESULT_CANNOT_BE_EMPTY = "Result cannot be empty";
        public const string EXPIRATION_MUST_BE_FUTURE = "Expiration must be in the future";
    }
    
    /// <summary>
    /// Location validation messages
    /// </summary>
    public static class Location
    {
        public const string CODE_CANNOT_BE_EMPTY = "Location code cannot be empty";
        public const string CODE_TOO_LONG = "Location code cannot exceed 20 characters";
        public const string NAME_CANNOT_BE_EMPTY = "Location name cannot be empty";
        public const string NAME_TOO_LONG = "Location name cannot exceed 100 characters";
    }
    
    /// <summary>
    /// Last Size validation messages
    /// </summary>
    public static class LastSize
    {
        public const string SIZE_VALUE_MUST_BE_POSITIVE = "Size value must be greater than zero";
        public const string SIZE_LABEL_REQUIRED = "Size label is required";
        public const string SIZE_LABEL_TOO_LONG = "Size label cannot exceed 20 characters";
        public const string ALREADY_DISCONTINUED = "Size is already discontinued";
        public const string ALREADY_ACTIVE = "Size is already active";
    }
    
    /// <summary>
    /// Last Name validation messages
    /// </summary>
    public static class LastName
    {
        public const string CUSTOMER_ID_MUST_BE_POSITIVE = "Customer ID must be positive";
        public const string LAST_CODE_CANNOT_BE_EMPTY = "Last code cannot be empty";
        public const string LAST_CODE_TOO_LONG = "Last code cannot exceed 50 characters";
        public const string DISCONTINUE_REASON_REQUIRED = "Discontinue reason is required when status is not Active";
        public const string CANNOT_TRANSFER_TO_SAME_CUSTOMER = "Cannot transfer to the same customer";
    }
    
    /// <summary>
    /// Last Model validation messages
    /// </summary>
    public static class LastModel
    {
        public const string LAST_ID_MUST_BE_POSITIVE = "Last ID must be positive";
        public const string MODEL_CODE_IS_REQUIRED = "Model code is required";
        public const string MODEL_CODE_CANNOT_BE_EMPTY = "Model code cannot be empty";
    }
    
    /// <summary>
    /// Inventory Stock validation messages
    /// </summary>
    public static class InventoryStock
    {
        public const string INITIAL_QUANTITY_CANNOT_BE_NEGATIVE = "Initial quantity cannot be negative";
        public const string ADJUSTMENT_QUANTITY_MUST_BE_POSITIVE = "Adjustment quantity must be positive";
        public const string INSUFFICIENT_STOCK = "Insufficient stock. Available: {0}, Requested: {1}";
        public const string CANNOT_DAMAGE_UNITS = "Cannot damage {0} units. Only {1} good units available.";
        public const string CANNOT_REPAIR_UNITS = "Cannot repair {0} units. Only {1} damaged units available.";
        public const string UNKNOWN_ADJUSTMENT_TYPE = "Unknown adjustment type: {0}";
        public const string RESERVE_QUANTITY_MUST_BE_POSITIVE = "Reserve quantity must be positive";
        public const string CANNOT_RESERVE_UNITS = "Cannot reserve {0} units. Only {1} available.";
        public const string RELEASE_QUANTITY_MUST_BE_POSITIVE = "Release quantity must be positive";
        public const string CANNOT_RELEASE_UNITS = "Cannot release {0} units. Only {1} reserved.";
    }
    
    /// <summary>
    /// Inventory Movement validation messages
    /// </summary>
    public static class InventoryMovement
    {
        public const string QUANTITY_MUST_BE_POSITIVE = "Quantity must be positive";
    }
}

namespace LastManagement.Infrastructure.Constants;

/// <summary>
/// Contains all database-related constants including table names, column names, and index names
/// </summary>
public static class DatabaseConstants
{
    /// <summary>
    /// Database schema names
    /// </summary>
    public static class Schema
    {
        public const string PUBLIC = "public";
    }

    /// <summary>
    /// Common column type definitions
    /// </summary>
    public static class ColumnTypes
    {
        public const string TIMESTAMP_TZ = "timestamptz";
        public const string TEXT = "text";
        public const string VARCHAR = "varchar";
        public const string INTEGER = "integer";
        public const string BIGINT = "bigint";
        public const string BOOLEAN = "boolean";
        public const string DECIMAL = "decimal";
    }

    /// <summary>
    /// Common SQL default values
    /// </summary>
    public static class DefaultValues
    {
        public const string NOW = "NOW()";
        public const string TRUE = "true";
        public const string FALSE = "false";
        public const string ZERO = "0";
        public const string ONE = "1";
    }

    /// <summary>
    /// Database views
    /// </summary>
    public static class Views
    {
        public const string INVENTORY_SUMMARY = "v_inventory_summary";
    }

    /// <summary>
    /// Purchase Orders table constants
    /// </summary>
    public static class PurchaseOrders
    {
        public const string TABLE_NAME = "purchase_orders";

        public static class Columns
        {
            public const string ORDER_ID = "order_id";
            public const string ORDER_NUMBER = "order_number";
            public const string LOCATION_ID = "location_id";
            public const string REQUESTED_BY = "requested_by";
            public const string DEPARTMENT = "department";
            public const string NOTES = "notes";
            public const string ADMIN_NOTES = "admin_notes";
            public const string CREATED_AT = "created_at";
            public const string REVIEWED_AT = "reviewed_at";
            public const string REVIEWED_BY = "reviewed_by";
            public const string VERSION = "version";
        }

        public static class Indexes
        {
            public const string UNIQUE_ORDER_NUMBER = "uq_purchase_orders_number";
            public const string STATUS = "idx_purchase_orders_status";
            public const string LOCATION = "idx_purchase_orders_location";
            public const string REQUESTED_BY = "idx_purchase_orders_requested_by";
            public const string CREATED_AT = "idx_purchase_orders_created_at";
        }
    }

    /// <summary>
    /// Purchase Order Items table constants
    /// </summary>
    public static class PurchaseOrderItems
    {
        public const string TABLE_NAME = "purchase_order_items";

        public static class Columns
        {
            public const string ITEM_ID = "item_id";
            public const string ORDER_ID = "order_id";
            public const string LAST_ID = "last_id";
            public const string SIZE_ID = "size_id";
            public const string QUANTITY_REQUESTED = "quantity_requested";
        }

        public static class Indexes
        {
            public const string LAST = "idx_purchase_order_items_last";
            public const string SIZE = "idx_purchase_order_items_size";
            public const string COMPOSITE = "idx_purchase_order_items_composite";
        }
    }

    /// <summary>
    /// Idempotency Keys table constants
    /// </summary>
    public static class IdempotencyKeys
    {
        public const string TABLE_NAME = "idempotency_keys";

        public static class Columns
        {
            public const string ID = "id";
            public const string KEY = "key";
            public const string RESULT = "result";
            public const string CREATED_AT = "created_at";
            public const string EXPIRES_AT = "expires_at";
        }

        public static class Indexes
        {
            public const string KEY = "idx_idempotency_keys_key";
            public const string EXPIRES_AT = "idx_idempotency_keys_expires_at";
        }
    }

    /// <summary>
    /// Locations table constants
    /// </summary>
    public static class Locations
    {
        public const string TABLE_NAME = "locations";

        public static class Columns
        {
            public const string LOCATION_ID = "location_id";
            public const string LOCATION_CODE = "location_code";
            public const string LOCATION_NAME = "location_name";
            public const string LOCATION_TYPE = "location_type";
            public const string IS_ACTIVE = "is_active";
            public const string CREATED_AT = "created_at";
        }

        public static class Indexes
        {
            public const string CODE = "idx_locations_code";
            public const string TYPE = "idx_locations_type";
            public const string ACTIVE = "idx_locations_active";
        }
    }

    /// <summary>
    /// Last Sizes table constants
    /// </summary>
    public static class LastSizes
    {
        public const string TABLE_NAME = "last_sizes";

        public static class Columns
        {
            public const string SIZE_ID = "size_id";
            public const string SIZE_VALUE = "size_value";
            public const string SIZE_LABEL = "size_label";
            public const string REPLACEMENT_SIZE_ID = "replacement_size_id";
            public const string CREATED_AT = "created_at";
            public const string UPDATED_AT = "updated_at";
        }

        public static class Indexes
        {
            public const string VALUE = "idx_last_sizes_value";
            public const string STATUS = "idx_last_sizes_status";
        }
    }

    /// <summary>
    /// Last Names table constants
    /// </summary>
    public static class LastNames
    {
        public const string TABLE_NAME = "last_names";

        public static class Columns
        {
            public const string LAST_ID = "last_id";
            public const string CUSTOMER_ID = "customer_id";
            public const string LAST_CODE = "last_code";
            public const string DESCRIPTION = "description";
            public const string DISCONTINUE_REASON = "discontinue_reason";
            public const string CREATED_AT = "created_at";
        }

        public static class Indexes
        {
            public const string CUSTOMER = "idx_last_names_customer";
            public const string CODE = "idx_last_names_code";
            public const string STATUS = "idx_last_names_status";
            public const string COMPOSITE = "idx_last_names_composite";
        }
    }

    /// <summary>
    /// Last Models table constants
    /// </summary>
    public static class LastModels
    {
        public const string TABLE_NAME = "last_models";

        public static class Columns
        {
            public const string MODEL_ID = "model_id";
            public const string LAST_ID = "last_id";
            public const string MODEL_CODE = "model_code";
            public const string DESCRIPTION = "description";
            public const string CREATED_AT = "created_at";
        }

        public static class Indexes
        {
            public const string LAST = "idx_last_models_last";
            public const string CODE = "idx_last_models_code";
            public const string COMPOSITE = "idx_last_models_composite";
        }
    }

    /// <summary>
    /// Customers table constants
    /// </summary>
    public static class Customers
    {
        public const string TABLE_NAME = "customers";

        public static class Columns
        {
            public const string CUSTOMER_ID = "customer_id";
            public const string CUSTOMER_NAME = "customer_name";
            public const string CONTACT_EMAIL = "contact_email";
            public const string CONTACT_PHONE = "contact_phone";
            public const string ADDRESS = "address";
            public const string CREATED_AT = "created_at";
        }

        public static class Indexes
        {
            public const string NAME = "idx_customers_name";
            public const string EMAIL = "idx_customers_email";
            public const string STATUS = "idx_customers_status";
        }
    }

    /// <summary>
    /// Inventory Stocks table constants
    /// </summary>
    public static class InventoryStocks
    {
        public const string TABLE_NAME = "inventory_stocks";

        public static class Columns
        {
            public const string STOCK_ID = "stock_id";
            public const string LAST_ID = "last_id";
            public const string SIZE_ID = "size_id";
            public const string LOCATION_ID = "location_id";
            public const string QUANTITY_GOOD = "quantity_good";
            public const string QUANTITY_DAMAGED = "quantity_damaged";
            public const string QUANTITY_RESERVED = "quantity_reserved";
            public const string LAST_UPDATED = "last_updated";
            public const string VERSION = "version";
        }

        public static class Indexes
        {
            public const string COMPOSITE_UNIQUE = "uq_inventory_stocks_composite";
            public const string LAST_SIZE = "idx_inventory_last_size";
            public const string LOCATION = "idx_inventory_location";
        }
    }

    /// <summary>
    /// Inventory Movements table constants
    /// </summary>
    public static class InventoryMovements
    {
        public const string TABLE_NAME = "inventory_movements";

        public static class Columns
        {
            public const string MOVEMENT_ID = "movement_id";
            public const string STOCK_ID = "stock_id";
            public const string MOVEMENT_TYPE = "movement_type";
            public const string QUANTITY = "quantity";
            public const string REFERENCE_TYPE = "reference_type";
            public const string REFERENCE_ID = "reference_id";
            public const string NOTES = "notes";
            public const string CREATED_AT = "created_at";
            public const string CREATED_BY = "created_by";
        }

        public static class Indexes
        {
            public const string STOCK = "idx_inventory_movements_stock";
            public const string TYPE = "idx_inventory_movements_type";
            public const string CREATED_AT = "idx_inventory_movements_created_at";
            public const string REFERENCE = "idx_inventory_movements_reference";
        }
    }

    /// <summary>
    /// Accounts table constants
    /// </summary>
    public static class Accounts
    {
        public const string TABLE_NAME = "accounts";

        public static class Columns
        {
            public const string ACCOUNT_ID = "account_id";
            public const string USERNAME = "username";
            public const string EMAIL = "email";
            public const string PASSWORD_HASH = "password_hash";
            public const string FULL_NAME = "full_name";
            public const string ROLE = "role";
            public const string IS_ACTIVE = "is_active";
            public const string CREATED_AT = "created_at";
            public const string LAST_LOGIN = "last_login";
            public const string VERSION = "version";
        }

        public static class Indexes
        {
            public const string USERNAME = "idx_accounts_username";
            public const string EMAIL = "idx_accounts_email";
            public const string ROLE = "idx_accounts_role";
        }
    }
}

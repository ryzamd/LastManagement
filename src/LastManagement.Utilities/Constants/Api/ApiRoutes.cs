namespace LastManagement.Api.Constants;

public static class ApiRoutes
{
    public const string API_VERSION = "1.0";
    public const string API_BASE = "api/v{version:apiVersion}";

    public static class PurchaseOrders
    {
        public const string BASE = "api/v{version:apiVersion}/purchase-orders";
        public const string BY_ID = "{id:int}";
        public const string CONFIRM = "{id:int}/confirm";
        public const string DENY = "{id:int}/deny";
        public const string PENDING = "pending";
        public const string SUMMARY = "summary";

        public const string FULL_BASE = "/api/v1/purchase-orders";
        public const string FULL_BY_ID_TEMPLATE = "/api/v1/purchase-orders/{0}";
        public const string FULL_WITH_ITEMS_TEMPLATE = "/api/v1/purchase-orders/{0}?$expand=items";
        public const string FULL_CONFIRM_TEMPLATE = "/api/v1/purchase-orders/{0}/confirm";
        public const string FULL_DENY_TEMPLATE = "/api/v1/purchase-orders/{0}/deny";
        public const string FULL_PAGINATION_TEMPLATE = "/api/v1/purchase-orders?limit={0}&after={1}";
        public const string FULL_LOCATION_FILTER_TEMPLATE = "/api/v1/locations/{0}/inventory";
        public const string FULL_ADJUST_TEMPLATE = "/api/v1/inventory/stocks/{0}/adjust";
        public const string FULL_MOVEMENT_TEMPLATE = "/api/v1/inventory/movements/{0}";

    }

    public static class Locations
    {
        public const string BASE = "api/v{version:apiVersion}/locations";
        public const string BY_ID = "{id:int}";

        public const string FULL_BASE = "/api/v1/locations";
        public const string FULL_BY_ID_TEMPLATE = "/api/v1/locations/{0}";
    }

    public static class Customers
    {
        public const string BASE = "api/v{version:apiVersion}/customers";
        public const string BY_ID = "{id:int}";

        public const string FULL_BASE = "/api/v1/customers";
        public const string FULL_BY_ID_TEMPLATE = "/api/v1/customers/{0}";
        public const string FULL_PAGINATION_TEMPLATE = "/api/v1/customers?limit={0}&after={1}";

        public static class QueryStrings
        {
            public const string FILTER_STATUS = "&$filter=status eq '{0}'";
            public const string ORDERBY = "&$orderby={0}";
        }
    }

    public static class LastNames
    {
        public const string BASE = "api/v{version:apiVersion}/last-names";
        public const string BY_ID = "{id:int}";

        public const string BY_CUSTOMER = "/api/v1/customers/{0}/lasts";
        public const string BY_CUSTOMER_WITH_PAGINATION = "/api/v1/customers/{0}/lasts?limit={1}&after={2}";

        public const string FULL_BASE = "/api/v1/last-names";
        public const string FULL_BY_ID_TEMPLATE = "/api/v1/last-names/{0}";
        public const string FULL_BY_LAST_WITH_MODELS = "/api/v1/last-names/{0}/models";
        public const string FULL_BY_LAST_WITH_INVENTORY = "/api/v1/last-names/{0}/inventory";
        public const string FULL_PAGINATION_TEMPLATE = "/api/v1/last-names?limit={0}&after={1}";
    }

    public static class LastSizes
    {
        public const string BASE = "api/v{version:apiVersion}/last-sizes";
        public const string BY_ID = "{id:int}";

        public const string FULL_BASE = "/api/v1/last-sizes";
        public const string FULL_BY_ID_TEMPLATE = "/api/v1/last-sizes/{0}";
        public const string FULL_PAGINATION_TEMPLATE = "/api/v1/last-sizes?limit={0}&after={1}";
    }

    public static class LastModels
    {
        public const string BASE = "api/v{version:apiVersion}/last-models";
        public const string BY_ID = "{id:int}";

        public const string BY_LAST_ID = "/api/v1/last-names/{0}/models";

        public const string FULL_BASE = "/api/v1/last-models";
        public const string FULL_BY_ID_TEMPLATE = "/api/v1/last-models/{0}";
        public const string FULL_BY_MODEL_ID = "/api/v1/last-models/{0}";
    }

    public static class Inventory
    {
        public const string STOCKS_BASE = "api/v{version:apiVersion}/inventory/stocks";
        public const string STOCKS = "stocks";

        public const string FULL_STOCKS_FILTER_TEMPLATE = "/api/v1/inventory/stocks?$filter=locationId eq {0}";
        public const string FULL_STOCKS_PAGINATION = "/api/v1/inventory/stocks?limit={0}&after={1}";
        public const string FULL_MOVEMENTS_PAGINATION = "/api/v1/inventory/movements?limit={0}&after={1}";
        public const string FULL_BY_ID_TEMPLATE = "/api/v1/inventory/stocks/{0}";
        public const string FULL_ADJUST_TEMPLATE = "/api/v1/inventory/stocks/{0}/adjust";
        public const string FULL_MOVEMENTS_BY_ID = "/api/v1/inventory/movements/{0}";
    }

    public static class InventoryStocks
    {
        public const string BASE = "api/v{version:apiVersion}/inventory/stocks";
        public const string BY_ID = "{id:int}";
        public const string ADJUST = "{id:int}/adjust";
        public const string TRANSFER = "transfer";

        public const string FULL_BASE = "/api/v1/inventory/stocks";
        public const string FULL_TRANSFER = "/api/v1/inventory/transfers";
        public const string FULL_ADJUSTMENT_BATCH = "/api/v1/inventory/adjustments/$batch";
        public const string FULL_LOW_STOCK = "/api/v1/inventory/low-stock";

        public const string FULL_BY_ID_TEMPLATE = "/api/v1/inventory/stocks/{0}";
    }

    public static class Authentication
    {
        public const string BASE = "api/v{version:apiVersion}/auth";
        public const string LOGIN = "login";
        public const string REFRESH = "refresh";
        public const string LOGOUT = "logout";
        public const string ME = "me";
        public const string CREATE_ADMIN = "create-admin";

        public const string FULL_BASE = "/api/v1/auth";
    }

    public static class QueryParameters
    {
        public const string EXPAND = "$expand";
        public const string FILTER = "$filter";
        public const string EXPAND_ITEMS = "items";
        public const string LIMIT = "limit";
        public const string AFTER = "after";
        public const string STATUS = "status";
    }
}

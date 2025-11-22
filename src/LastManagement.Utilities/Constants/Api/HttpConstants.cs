namespace LastManagement.Api.Constants;

public static class HttpConstants
{
    public static class ContentTypes
    {
        public const string APPLICATION_JSON = "application/json";
        public const string APPLICATION_PROBLEM_JSON = "application/problem+json";
        public const string BEARER = "Bearer";
    }

    public static class Methods
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string PATCH = "PATCH";
        public const string DELETE = "DELETE";
    }

    public static class Headers
    {
        public const string IDEMPOTENCY_KEY = "Idempotency-Key";
        public const string LOCATION = "Location";
        public const string ETAG = "ETag";
        public const string IF_NONE_MATCH = "If-None-Match";
        public const string CONTENT_TYPE = "Content-Type";
        public const string IF_MATCH = "If-Match";
        public const string X_TOTAL_COUNT = "X-Total-Count";
        public const string CACHE_CONTROL = "Cache-Control";
    }
}

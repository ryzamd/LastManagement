namespace LastManagement.Api.Constants;

public static class ProblemDetailsConstants
{
    public static class Types
    {
        public const string BAD_REQUEST = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        public const string NOT_FOUND = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        public const string INTERNAL_SERVER_ERROR = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

        // Custom problem types
        public const string VALIDATION_ERROR = "http://localhost:5000/problems/validation-error";
        public const string NOT_FOUND_ERROR = "http://localhost:5000/problems/not-found";
        public const string ORDER_ALREADY_REVIEWED = "http://localhost:5000/problems/order-already-reviewed";
        public const string DUPLICATE_RESOURCE = "http://localhost:5000/problems/duplicate-resource";
        public const string CONFLICT = "http://localhost:5000/problems/conflict";
        public const string PRECONDITION_FAILED = "http://localhost:5000/problems/precondition-failed";
    }

    public static class Titles
    {
        public const string BAD_REQUEST = "Bad Request";
        public const string NOT_FOUND = "Not Found";
        public const string INTERNAL_SERVER_ERROR = "Internal Server Error";
        public const string VALIDATION_ERROR = "Validation Error";
        public const string ONE_OR_MORE_VALIDATION_ERRORS = "One or more validation errors occurred";
        public const string ORDER_ALREADY_REVIEWED = "Order Already Reviewed";
        public const string DUPLICATE_RESOURCE = "Duplicate Resource";
        public const string CONFLICT = "Conflict";
        public static string PRECONDITION_FAILED = "Precondition Failed";
    }

    public static class Details
    {
        public const string IF_MATCH_HEADER_REQUIRED_ETAG = "If-Match header with valid ETag is required";
    }
}

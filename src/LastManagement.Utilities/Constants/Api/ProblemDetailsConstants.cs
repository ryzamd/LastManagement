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
        public const string PRECONDITION_REQUIRED = "http://localhost:5000/problems/precondition-required";
        public const string INSUFFICIENT_STOCK = "http://localhost:5000/problems/insufficient-stock";
        public const string INTERNAL_ERROR = "http://localhost:5000/problems/internal-error";

        public const string RFC_BAD_REQUEST = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
        public const string RFC_INTERNAL_SERVER_ERROR = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
        public const string RFC_NOT_FOUND = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
        public const string RFC_CONFLICT = "https://tools.ietf.org/html/rfc9110#section-15.5.10";
        public const string RFC_PRECONDITION_FAILED = "https://tools.ietf.org/html/rfc9110#section-15.5.13";

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
        public const string PRECONDITION_FAILED = "Precondition Failed";
        public const string PRECONDITION_REQUIRED = "Precondition Required";
        public const string INSUFFICIENT_STOCK = "Insufficient Stock";
        public const string CONCURRENCY_CONFLICT = "Concurrency Conflict";
    }

    public static class Details
    {
        public const string IF_MATCH_HEADER_REQUIRED_ETAG = "If-Match header with valid ETag is required";
        public const string IF_MATCH_REQUIRED_FOR_UPDATES = "If-Match header is required for updates";

        public const string ETAG_MISMATCH = "ETag mismatch. Resource was modified.";

        public const string INVALID_FIELD_VALUES = "One or more fields contain invalid values";
        public const string RETRIEVING_LAST_MODELS_ERROR = "An error occurred while retrieving last models";
        public const string LAST_ID_MUST_BE_POSITIVE_INTERGER = "Last ID must be a positive integer";
        public const string CREATE_LAST_NAME_ERROR = "An error occurred while creating the last name";
    }
}

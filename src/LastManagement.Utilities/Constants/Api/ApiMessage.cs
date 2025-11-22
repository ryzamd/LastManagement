namespace LastManagement.Api.Constants
{
    public static class ApiMessage
    {
        public static class Errors
        {

            public static class Authenication
            {
                public const string USER_NAME_CONFLICT = "Username already exists";
            }

            public static class Customer
            {
                public const string CUSTOMER_EXISTED = "Customer already exists";
                public const string CUSTOMER_NOT_FOUND = "Customer not found";
                public const string CUSTOMER_MODIFIED_BY_ANOTHER = "Customer modified by another user";
                public const string CUSTOMER_HAS_ASSOCIATED_LASTS = "Customer has associated lasts";
            }
        }

        public static class Success
        {
            public static class Authenication
            {
                public const string CREATE_ADMIN_ACCOUNT_SUCCESS = "Admin created";
            }
        }
    }
}

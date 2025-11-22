namespace LastManagement.Utilities.Constants.Global
{
    public static class ConflictMessages
    {
        public static class Reasons
        {
            public const string DUPLICATE_LAST_CODE = "duplicate-last-code";
            public const string HAS_DEPENDENCIES = "has-dependencies";
            public const string HAS_INVENTORY = "has-inventory";
            public const string DUPLICATE_SIZE_VALUE = "duplicate-size-value";
        }

        public static class Suggestions
        {
            public const string DELETE_REASSIGN_RECORDS = "Delete or reassign all associated records before deleting the last name";
            public const string CHANGE_STATUS_OBSOLETE = "Change last name status to 'Obsolete' instead using PATCH";
            public const string REMOVE_INVENTORY_RECORDS = "Remove all inventory records using this size before deleting";
            public const string DISCONTINUE_SIZE_INSTEAD = "Discontinue the size instead using PATCH with status: Discontinued";
        }
    }
}

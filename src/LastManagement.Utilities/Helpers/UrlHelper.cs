namespace LastManagement.Api.Global.Helpers
{
    public static class UrlHelper
    {
        public static string FormatResourceUrl(string template, params object[] args) => string.Format(template, args);

        public static string FormatNextLink(string template, int limit, int afterId) => string.Format(template, limit, afterId);

        public static string FormatInstancePath(string template, int id) => string.Format(template, id);
    }
}

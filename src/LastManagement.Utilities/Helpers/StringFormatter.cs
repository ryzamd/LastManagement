namespace LastManagement.Utilities.Helpers;

public static class StringFormatter
{
    public static string FormatError(string template, params object[] args) => string.Format(template, args);
    public static string FormatValidation(string template, params object[] args) => string.Format(template, args);
    public static string FormatMessage(string template, params object[] args) => string.Format(template, args);
}
using System.Security.Cryptography;
using System.Text;

namespace LastManagement.Api.Global.Helpers;

public static class ETagHelper
{
    public static string Generate(int version)
    {
        var bytes = Encoding.UTF8.GetBytes(version.ToString());
        var hash = MD5.HashData(bytes);
        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    public static int? ParseVersion(string? etag)
    {
        if (string.IsNullOrEmpty(etag))
            return null;

        etag = etag.Trim('"');

        try
        {
            var bytes = Convert.FromBase64String(etag);
            var versionStr = Encoding.UTF8.GetString(bytes);
            return int.TryParse(versionStr, out var version) ? version : null;
        }
        catch
        {
            return null;
        }
    }
}
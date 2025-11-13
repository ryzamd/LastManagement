using System.Security.Cryptography;

namespace LastManagement.Api.Global.Middleware;

public sealed class ETagMiddleware
{
    private readonly RequestDelegate _next;

    public ETagMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var response = context.Response;
        var originalBodyStream = response.Body;

        using var responseBody = new MemoryStream();
        response.Body = responseBody;

        await _next(context);

        if (response.StatusCode == StatusCodes.Status200OK &&
            context.Request.Method == HttpMethods.Get)
        {
            var body = await FormatResponseAsync(responseBody);
            var etag = GenerateETag(body);

            response.Headers.ETag = etag;

            if (context.Request.Headers.IfNoneMatch == etag)
            {
                response.StatusCode = StatusCodes.Status304NotModified;
                response.Body = originalBodyStream;
                response.ContentLength = 0;
                return;
            }
        }

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
        response.Body = originalBodyStream;
    }

    private static async Task<byte[]> FormatResponseAsync(Stream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var buffer = new byte[responseBody.Length];
        await responseBody.ReadAsync(buffer);
        return buffer;
    }

    private static string GenerateETag(byte[] data)
    {
        var hash = MD5.HashData(data);
        return $"\"{Convert.ToBase64String(hash)}\"";
    }
}
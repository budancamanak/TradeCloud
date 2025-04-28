using System.Net;
using Microsoft.AspNetCore.Http;

namespace Common.Web.Http;

public static class HttpContextExtensions
{
    public static string? CurrentUser(this IHttpContextAccessor contextAccessor)
    {
        return contextAccessor.HttpContext?.Items.FirstOrDefault(f => f.Key.ToString() == "CurrentUser")
            .Value?.ToString();
    }

    public static string GetClientIp(this IHttpContextAccessor contextAccessor)
    {
        var remoteIpAddress =
            contextAccessor.GetClientIpFromHeaders() ?? contextAccessor.HttpContext?.Connection.RemoteIpAddress;
        var result = "";
        if (remoteIpAddress == null) return result;
        if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        result = remoteIpAddress.ToString();

        return result;
    }


    public static IPAddress? GetClientIpFromHeaders(this IHttpContextAccessor contextAccessor)
    {
        try
        {
            var ip = GetHeaderValueAs<string>(contextAccessor, "X-Forwarded-For")?.SplitCsv().FirstOrDefault();

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (string.IsNullOrWhiteSpace(ip) && contextAccessor.HttpContext?.Connection.RemoteIpAddress != null)
                ip = contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrWhiteSpace(ip))
                ip = GetHeaderValueAs<string>(contextAccessor, "REMOTE_ADDR");

            if (string.IsNullOrWhiteSpace(ip))
                throw new Exception("Unable to determine caller's IP.");
            return IPAddress.Parse(ip);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    private static T? GetHeaderValueAs<T>(IHttpContextAccessor contextAccessor, string headerName)
    {
        if (!(contextAccessor.HttpContext?.Request.Headers.TryGetValue(headerName, out var values) ?? false))
            return default(T);
        var rawValues = values.ToString();

        if (!string.IsNullOrWhiteSpace(rawValues))
            return (T)Convert.ChangeType(values.ToString(), typeof(T));

        return default(T);
    }
}

internal static class StringExtensions
{
    public static List<string> SplitCsv(this string csvList)
    {
        if (string.IsNullOrWhiteSpace(csvList))
            return [];

        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable()
            .Select(s => s.Trim())
            .ToList();
    }
}
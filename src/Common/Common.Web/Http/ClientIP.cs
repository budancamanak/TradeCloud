using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Common.Web.Http;

public class ClientIP(IHttpContextAccessor contextAccessor)
{
    public string GetClientIPv1(bool tryUseXForwardHeader = true)
    {
        string ip = null;

        // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

        // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
        // for 99% of cases however it has been suggested that a better (although tedious)
        // approach might be to read each IP from right to left and use the first public IP.
        // http://stackoverflow.com/a/43554000/538763
        //
        if (tryUseXForwardHeader)
            ip = GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault();

        // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
        if (ip.IsNullOrWhitespace() && contextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
            ip = contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        if (ip.IsNullOrWhitespace())
            ip = GetHeaderValueAs<string>("REMOTE_ADDR");

        // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

        if (ip.IsNullOrWhitespace())
            throw new Exception("Unable to determine caller's IP.");

        return ip;
    }

    public T GetHeaderValueAs<T>(string headerName)
    {
        StringValues values;

        if (contextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
        {
            string rawValues = values.ToString(); // writes out as Csv when there are multiple.

            if (!rawValues.IsNullOrWhitespace())
                return (T)Convert.ChangeType(values.ToString(), typeof(T));
        }

        return default(T);
    }


    public string GetClientIPv2()
    {
        IPAddress remoteIpAddress = contextAccessor.HttpContext.Connection.RemoteIpAddress;
        string result = "";
        if (remoteIpAddress != null)
        {
            // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
            // This usually only happens when the browser is on the same machine as the server.
            if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList
                    .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }

            result = remoteIpAddress.ToString();
        }

        return result;
    }
}

public static class HttpContextExtensions
{
    public static string? CurrentUser(this IHttpContextAccessor contextAccessor)
    {
        return contextAccessor.HttpContext?.Items.FirstOrDefault(f => f.Key.ToString() == "CurrentUser")
            .Value?.ToString();
    }
}

internal static class StringExtensions
{
    public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
    {
        if (string.IsNullOrWhiteSpace(csvList))
            return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable<string>()
            .Select(s => s.Trim())
            .ToList();
    }

    public static bool IsNullOrWhitespace(this string s)
    {
        return String.IsNullOrWhiteSpace(s);
    }
}
using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;

namespace Project.Shared;

public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;


    public SecurityMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;

        LogComponentVersions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew(); // Track execution time

        //if (!context.Request.IsHttps && !_environment.IsDevelopment())
        //{
        //    var httpsUrl = $"https://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
        //    context.Response.Redirect(httpsUrl, permanent: true);
        //    return;
        //}

        //context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
        //context.Response.Headers.Remove("Server");
        //context.Response.Headers.Remove("X-Powered-By");
        SetSecurityHeaders(context);


        if (ContainsMaliciousInput(context))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("Malicious input detected.");
            return;
        }


        if (!IsFileUploadSafe(context))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("Invalid file upload.");
            return;
        }

        if (ContainsUnsafeUrls(context))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("Unsafe URLs detected.");
            return;
        }

        LogRequest(context);
        await _next(context);
        LogResponse(context, stopwatch.ElapsedMilliseconds); // Log response time
    }



    private void SetSecurityHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers["X-Content-Type-Options"] = "nosniff";
        //headers["X-Frame-Options"] = "DENY";
        headers["X-XSS-Protection"] = "1; mode=block";
        //headers["Referrer-Policy"] = "no-referrer";
        //headers["Permissions-Policy"] = "geolocation=(), camera=()";
        //headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; object-src 'none';";
    }



    private bool ContainsMaliciousInput(HttpContext context)
    {
        var inputs = new[]
        {
            context.Request.QueryString.Value,
            context.Request.Headers.ToString(),
            context.Request.Cookies.ToString()
        };

        if (context.Request.HasFormContentType)
        {
            var form = context.Request.Form;
            foreach (var field in form)
            {
                inputs = Append(inputs, field.Value);
            }
        }

        var sqlInjectionPattern = new Regex(@"(\%27)|(\')|(\-\-)|(\%23)|(#)", RegexOptions.IgnoreCase);
        var xssPattern = new Regex(@"<.*?>", RegexOptions.IgnoreCase);

        foreach (var input in inputs)
        {
            if (sqlInjectionPattern.IsMatch(input) || xssPattern.IsMatch(input))
            {
                Log.Warning($"Malicious input detected: {input}");
                return true;
            }
        }

        return false;
    }



    private bool IsFileUploadSafe(HttpContext context)
    {
        if (context.Request.HasFormContentType)
        {
            var form = context.Request.Form;

            foreach (var file in form.Files)
            {
                if (file.Length > 100 * 1024 * 1024)
                {
                    Log.Warning($"File too large: {file.FileName}");
                    return false;
                }

                var allowedExtensions = new[] { ".jpg", ".png", ".pdf", ".docx", ".jpeg" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (Array.IndexOf(allowedExtensions, extension) < 0)
                {
                    Log.Warning($"Disallowed file type: {file.FileName}");
                    return false;
                }
            }
        }

        return true;
    }

    private bool ContainsUnsafeUrls(HttpContext context)
    {
        var inputs = new[]
        {
            context.Request.QueryString.Value,
            context.Request.Headers.ToString(),
            context.Request.Cookies.ToString()
        };

        foreach (var input in inputs)
        {
            var urls = ExtractUrls(input);
            foreach (var url in urls)
            {
                if (IsInternalIp(url))
                {
                    Log.Warning($"SSRF attempt detected: {url}");
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsInternalIp(string url)
    {
        try
        {
            var uri = new Uri(url);
            var host = uri.Host;

            if (IPAddress.TryParse(host, out var ip))
            {
                var bytes = ip.GetAddressBytes();
                switch (bytes[0])
                {
                    case 10:
                    case 127:
                        return true;
                    case 172:
                        return bytes[1] >= 16 && bytes[1] <= 31;
                    case 192:
                        return bytes[1] == 168;
                    default:
                        return false;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private void LogRequest(HttpContext context)
    {
        var user = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous";
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var ip = context.Connection.RemoteIpAddress?.ToString();

        Log.Information($"Request: {context.Request.Method} {context.Request.Path}, User: {user}, IP: {ip}, User-Agent: {userAgent}, Timestamp: {DateTime.UtcNow}");
    }

    private void LogResponse(HttpContext context, long executionTimeMs)
    {
        var statusCode = context.Response.StatusCode;
        Log.Information($"Response: {statusCode}, Execution Time: {executionTimeMs}ms, Timestamp: {DateTime.UtcNow}");
    }


    private void LogComponentVersions()
    {
        Log.Information($"ASP.NET Core Version: {Environment.Version}");
    }

    private static string[] Append(string[] array, params string[] items)
    {
        var result = new string[array.Length + items.Length];
        array.CopyTo(result, 0);
        items.CopyTo(result, array.Length);
        return result;
    }

    private static string[] ExtractUrls(string input)
    {
        var urls = new List<string>();
        var regex = new Regex(@"(http|https):\/\/[^\s/$.?#].[^\s]*", RegexOptions.IgnoreCase);
        var matches = regex.Matches(input);
        foreach (Match match in matches)
        {
            urls.Add(match.Value);
        }
        return urls.ToArray();
    }
}

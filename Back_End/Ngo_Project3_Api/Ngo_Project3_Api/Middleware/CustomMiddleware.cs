using System.Threading.Tasks;

namespace Ngo_Project3_Api.Middleware
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-Key";
        private const string ApiKeyValue = "ABCNHAKQWRUQNAOQUTNASFAKKJQ123";

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Logic trước khi chuyển đến controller
            Console.WriteLine($"Request Path: {context.Request.Path}");

            // Kiểm tra Header
            // Kiểm tra header X-API-Key
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key is missing!");
                return;
            }

            // Kiểm tra giá trị của API Key
            if (!string.Equals(extractedApiKey, ApiKeyValue))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key!");
                return;
            }

            // Gọi middleware tiếp theo trong pipeline
            await _next(context);

            // Logic sau khi xử lý response
            Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
        }
    }

    // Extension method để dễ dàng thêm Middleware vào Pipeline
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();
        }
    }
}

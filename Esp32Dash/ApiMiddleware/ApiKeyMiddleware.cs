namespace Esp32Dash.ApiMiddleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER = "X-Api-Key";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Validate for POST
            if (context.Request.Path.StartsWithSegments("/data") &&
                context.Request.Method == "POST")
            {
                if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "API Key is missing" });
                    return;
                }

                var apiSettings = context.RequestServices.GetRequiredService<IConfiguration>();
                var apiKey = apiSettings.GetValue<string>("ApiSettings:ApiKey");

                if (!apiKey.Equals(extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKey(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}

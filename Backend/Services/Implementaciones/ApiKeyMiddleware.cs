

using Backend.Services.Interfaces;

namespace Backend.Services.Implementations
{
    public class ApiKeyMiddleware : IApiKeyMiddleware
    {

        const string APIKEYNAME = "ApiKey";
        RequestDelegate _requestDelegate;

        public ApiKeyMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedValue))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("No contiene APiKey");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);

            if (!apiKey.Equals(extractedValue))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("APiKey incorrecto");
                return;
            }

            await _requestDelegate(context);


        }
    }
}
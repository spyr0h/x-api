namespace XApi.API.Filter;

public class PrivateApiKeyAuthorizationFilter : IEndpointFilter
{
    private readonly string _apiKey;

    public PrivateApiKeyAuthorizationFilter()
    {
        _apiKey = Environment.GetEnvironmentVariable("PRIVATE_API_KEY") ?? string.Empty;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (string.IsNullOrEmpty(_apiKey) ||
            !context.HttpContext.Request.Headers.TryGetValue("Authorization", out var providedApiKey) ||
            !string.Equals(providedApiKey, $"Bearer {_apiKey}", StringComparison.Ordinal))
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}
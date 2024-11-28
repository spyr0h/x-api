namespace XApi.API.Filter;

public class PublicApiKeyAuthorizationFilter : IEndpointFilter
{
    private readonly string _apiKey;

    public PublicApiKeyAuthorizationFilter()
    {
        _apiKey = Environment.GetEnvironmentVariable("PUBLIC_API_KEY") ?? string.Empty;
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
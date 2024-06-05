namespace Dima.Api;

public static class ApiConfiguration
{
    public const string CorsPolicyName = "wasm";
    public static string StripeApiKey { get; set; } = string.Empty;
}
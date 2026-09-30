internal static class AdminIdentity
{
    public const string DefaultUserId = "00000000-0000-4000-8000-000000000001";

    /// <summary>生产应通过 Admin:PrincipalId 配置；默认值仅用于联调兼容。</summary>
    public static string UserId { get; private set; } = DefaultUserId;

    public static void Configure(string? configuredPrincipalId)
    {
        if (string.IsNullOrWhiteSpace(configuredPrincipalId)) return;
        if (!Guid.TryParse(configuredPrincipalId, out var parsed) || parsed == Guid.Empty) return;
        UserId = parsed.ToString("D");
    }
}

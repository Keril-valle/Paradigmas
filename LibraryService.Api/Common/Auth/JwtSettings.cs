namespace LibraryService.Api.Common.Auth;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;
}
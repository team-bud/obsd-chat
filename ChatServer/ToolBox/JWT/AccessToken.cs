using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ToolBox.JWT;

// Value
public readonly record struct AccessToken
{
    // core
    public required string RawValue { get; init; }
    public required SymmerticKey symKey { get; init; }
    public required string Issuer { get; init; }

    // operator
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(RawValue))
        {
            Console.WriteLine("Token is empty");
            return false;
        }

        var handler = new JwtSecurityTokenHandler();

        // 토큰 형식 자체가 맞는지 확인
        if (!handler.CanReadToken(RawValue))
        {
            Console.WriteLine("Not a valid JWT format");
            return false;
        }

        // 검증 파라미터 (발급 시 사용했던 값과 동일해야 함)
        var parameters = new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            // 👇 키는 SymmerticKey.Random()이나 SymmerticKey.Create(...)로 발급했던 동일 키여야 합니다
            IssuerSigningKey = symKey.RawValue,

            ValidateIssuer = true,
            ValidIssuer = Issuer,

            ValidateAudience = true,
            ValidAudience = Issuer,

            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,

            ValidTypes = new[] { "JWT" }
        };

        try
        {
            handler.ValidateToken(RawValue, parameters, out var securityToken);

            // 알고리즘 확인 (안전성 위해)
            if (securityToken is JwtSecurityToken jwt &&
                jwt.Header.Alg == SecurityAlgorithms.HmacSha256)
            {
                return true;
            }

            Console.WriteLine("Unexpected signing algorithm");
            return false;
        }
        catch (SecurityTokenExpiredException)
        {
            Console.WriteLine("Token is expired");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Validation failed: {ex.Message}");
            return false;
        }
    }

    public string? ExtractSubject()
    {
        if (string.IsNullOrWhiteSpace(RawValue))
            return null;

        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(RawValue))
            return null;

        var parameters = new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = symKey.RawValue,

            ValidateIssuer = true,
            ValidIssuer = Issuer,

            ValidateAudience = true,
            ValidAudience = Issuer,

            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = handler.ValidateToken(RawValue, parameters, out var securityToken);

            if (securityToken is JwtSecurityToken jwt &&
                jwt.Header.Alg == SecurityAlgorithms.HmacSha256)
            {
                // sub 클레임 추출
                return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
    
    public TokenSet PackageWithRandomRefreshToken()
    {
        return new TokenSet
        {
            Access = this,
            Refresh = RefreshToken.Random()
        };
    }
}
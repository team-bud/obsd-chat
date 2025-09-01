using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ToolBox.JWT;

public readonly record struct TokenInfo()
{
    // core
    public required string Subject { get; init; }
    public required string Issuer { get; init; } // 토큰을 발행한 주체(인증서버)
    public required string Audience { get; init; } // 토큰을 사용하는 주체(리소스서버)
    public required DateTime? NotBefore { get; init; } = null; // 토큰 시작 시간
    public required DateTime? Expires { get; init; } = null; // 토큰 만료 시간
    
    // operator
    public AccessToken SignBy(SymmerticKey symmerticKey)
    {
        // 1) 클레임 구성
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, Subject),
            new Claim(JwtRegisteredClaimNames.Iss, Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, Audience),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 2) 서명 자격 증명
        var creds = new SigningCredentials(symmerticKey.RawValue, SecurityAlgorithms.HmacSha256);

        // 3) 토큰 객체 생성
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: NotBefore ?? DateTime.UtcNow,
            expires: Expires ?? DateTime.UtcNow.AddHours(1), // 기본 1시간 유효
            signingCredentials: creds
        );

        // 4) 문자열 토큰으로 직렬화
        var handler = new JwtSecurityTokenHandler();
        var rawValue = handler.WriteToken(token);

        // 5) AccessToken으로 감싸 반환
        return new AccessToken
        {
            RawValue = rawValue,
            Issuer = Issuer,
            symKey = symmerticKey
        };
    }
}
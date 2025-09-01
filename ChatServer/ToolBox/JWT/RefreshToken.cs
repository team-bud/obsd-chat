using System.Security.Cryptography;

namespace ToolBox.JWT;


// Value
public readonly record struct RefreshToken
{
    // core
    public required string RawValue { get; init; }

    public static RefreshToken Random(int bytes = 32)
    {
        // buffer
        Span<byte> buffer = stackalloc byte[bytes];
        RandomNumberGenerator.Fill(buffer);

        // tokenValue
        var tokenValue = Convert.ToBase64String(buffer);

        // refreshToken
        var refreshToken = new RefreshToken { RawValue = tokenValue };
        
        // return
        return refreshToken;
    }
}
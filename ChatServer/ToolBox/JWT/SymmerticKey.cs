using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace ToolBox.JWT;


// Value
public readonly record struct SymmerticKey
{
    // core
    public required SymmetricSecurityKey RawValue { get; init; }

    public static SymmerticKey Random(int keySizeInBytes = 32)
    {
        // 32바이트 = 256비트 → HS256 알고리즘에 충분
        var buffer = new byte[keySizeInBytes];
        RandomNumberGenerator.Fill(buffer);

        var rawValue = new SymmetricSecurityKey(buffer);
        return new SymmerticKey { RawValue = rawValue };
    }
}
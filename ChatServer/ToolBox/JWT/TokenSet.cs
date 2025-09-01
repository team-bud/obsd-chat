namespace ToolBox.JWT;


// Value
public readonly record struct TokenSet
{
    // core
    public required AccessToken Access { get; init; }
    public required RefreshToken Refresh { get; init; }
}
using System.Collections.Concurrent;
using ToolBox.JWT;

namespace ServerCore;


// Object
public class UserCard
{
    // core
    public UserCard(ChatServer ownerRef, string email, string password)
    {
        OwnerRef = ownerRef;
        Email = email;
        Passsword = password;
        
        UserCardManager.Register(this);
    }

    
    // state
    public ID Id { get; } = ID.Random();
    private ChatServer OwnerRef { get; }

    public string Email { get; set; }
    public string Passsword { get; set; }

    public string? RefreshToken { get; private set; } = null;

    public void SetRefreshToken(AccessToken accessToken, string refreshToken)
    {
        if (accessToken.IsValid())
        {
            this.RefreshToken = refreshToken;
        }
        else
        {
            Console.WriteLine("Invalid access token - SetRefreshToken() cancel");
        }
    }
    
    
    
    // action
    public void RemoveUser()
    {
        Console.WriteLine("사용자를 ChatServer에서 제거합니다.");
    }
    
    
    // value
    public readonly record struct ID
    {
        // core
        public required Guid RawValue { get; init; }
        
        public static ID Random() => new ID { RawValue = Guid.NewGuid() };
        
        // operator
        public bool IsExist => UserCardManager.Get(this) is not null;
        public UserCard? Ref => UserCardManager.Get(this);
    }
}


// ObjectManager
internal static class UserCardManager
{
    // core
    internal static ConcurrentDictionary<UserCard.ID, UserCard> Container { get; } = [];
    internal static void Register(UserCard obj)
    {
        Container.TryAdd(obj.Id, obj);
    }
    internal static void Unregister(UserCard.ID id)
    {
        Container.TryRemove(id, out _);
    }

    internal static UserCard? Get(UserCard.ID id)
    {
        return Container.GetValueOrDefault(id);
    }
}
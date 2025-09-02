using System.Collections.Concurrent;
using ToolBox.JWT;
using ToolBox.Ticket;

namespace ServerCore;


// Object
public class ChatServer
{
    // core
    
    
    // state
    internal ID Id { get; } = ID.Random();
    
    private const string Owner = "mandoo";
    private readonly SymmerticKey _symmerticKey = SymmerticKey.Random();
    
    public HashSet<UserCard.ID> Users { get; } = [];
    public TokenSet? GetAuthToken(string email, string password)
    {
        // email, password 검증
        var isUserExist = Users
            .Where(u => u.Ref != null)
            .Select(u => u.Ref)
            .Any(us => us!.Email == email && us.Passsword == password);
        
        if (!isUserExist)
        {
            Console.WriteLine("User not found");
            return null;
        }
        
        // token 발급
        var tokenInfo = new TokenInfo
        {
            Subject = email,
            Issuer = Owner,
            Audience = Owner,
            NotBefore = DateTimeOffset.Now.UtcDateTime,
            Expires = DateTimeOffset.Now.AddDays(1).UtcDateTime
        };

        // accessToken + RefreshToken
        var tokenSet = tokenInfo
            .SignBy(_symmerticKey)
            .PackageWithRandomRefreshToken();
        
        return tokenSet;
    }
    
    public Queue<CreateUser> UserTickets { get; private set; } = [];
    public void AddTicket(CreateUser ticket) => UserTickets.Enqueue(ticket);

    
    public HashSet<ChatRoom.ID> Rooms { get; } = [];
    
    public Queue<CreateRoom> RoomTickets { get; private set; } = [];
    public void AddTicket(CreateRoom ticket) => RoomTickets.Enqueue(ticket);
    
    
    // action
    public void CreateUsers()
    {
        // mutate
        while (UserTickets.Count > 0)
        {
            var ticket = UserTickets.Dequeue();


            var isUserExist = Users
                .Select(u => u.Ref)
                .OfType<UserCard>()
                .Any(userCardRef => userCardRef.Email == ticket.Email);
            
            if (isUserExist)
            {
                Console.WriteLine($"Error: User with email {ticket.Email} already exists.");
                return;
            }

            
            var userCardRef = new UserCard(this, ticket.Email, ticket.Password);
            Users.Add(userCardRef.Id);
        }
        
        Console.WriteLine($"현재 Users 수 : {Users.Count}");
    }

    public void CreateRooms()
    {
        // mutate
        while (RoomTickets.Count > 0)
        {
            var ticket = RoomTickets.Dequeue();

            var accessToken = ticket.AccessToken;
            if (accessToken.ExtractSubject() is not string userEmail)
            {
                Console.WriteLine("Error: Invalid access token");
                continue;
            }

            var chatRoomRef = new ChatRoom(Id, ticket, userEmail);
            Rooms.Add(chatRoomRef.Id);
        }
        
        Console.WriteLine($"현재 Room 수: {Rooms.Count}");
    }
    
    
    
    // value
    public readonly record struct ID
    {
        // core
        public required Guid Value { get; init; }
        
        public static ID Random() => new ID { Value = Guid.NewGuid(), };
        
        // operator
        public ChatServer? Ref()
        {
            return ChatServerManager.Get(this);
        }
    }
    public readonly record struct CreateUser(): ITicket
    {
        public Guid Id { get; } = Guid.NewGuid();
        public required string Email { get; init; }
        public required string Password { get; init; }
    }

    public readonly record struct CreateRoom(): ITicket
    {
        public Guid Id { get; } = Guid.NewGuid();
        public AccessToken AccessToken { get; init; }
    }
}


// ObjectManager
internal static class ChatServerManager
{
    // core
    internal static ConcurrentDictionary<ChatServer.ID, ChatServer> Container = [];

    internal static void Register(ChatServer obj)
    {
        Container.TryAdd(obj.Id, obj);
    }

    internal static ChatServer? Get(ChatServer.ID id)
    {
        Container.TryGetValue(id, out var obj);
        return obj;
    }
}
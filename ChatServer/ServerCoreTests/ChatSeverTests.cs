using Xunit;
using ServerCore;
using ToolBox.JWT;

namespace ServerCoreTests.ChatServerTests;

public class CreateUsers
{
    // target
    private readonly ChatServer chatServerRef = new();

    // Tests
    [Fact]
    public void RemoveUserTicket()
    {
        // given
        Assert.Empty(chatServerRef.UserTickets);

        var newTicket = new ChatServer.CreateUser
        {
            Email = "sample@example.com",
            Password = "123456"
        };
        
        chatServerRef.AddTicket(newTicket);

        Assert.Single(chatServerRef.UserTickets);
        
        // when
        chatServerRef.CreateUsers();

        // then
        Assert.Empty(chatServerRef.UserTickets);
    }

    [Fact]
    public void CreateUserCard()
    {
        // given
        Assert.Empty(chatServerRef.Users);
        
        var newTicket = new ChatServer.CreateUser
        {
            Email = "sample@example.com",
            Password = "123456"
        };
        
        chatServerRef.AddTicket(newTicket);

        Assert.Single(chatServerRef.UserTickets);
        
        // when
        chatServerRef.CreateUsers();
        
        // then
        Assert.Single(chatServerRef.Users);
        
        var userCard = chatServerRef.Users.First();
        Assert.True(userCard.IsExist);
    }
}


public class CreateRooms
{
    private readonly ChatServer chatServerRef = new();

    [Fact]
    public void RemoveCreateRoomTicket()
    {
        // given
        var (userTokenSet, _) = chatServerRef.CreateTestUser();

        var createRoomTicket = new ChatServer.CreateRoom
        {
            AccessToken = userTokenSet.Access
        };
        
        chatServerRef.AddTicket(createRoomTicket);
        
        Assert.Empty(chatServerRef.Rooms);
        Assert.Single(chatServerRef.RoomTickets);
            
        // when
        chatServerRef.CreateRooms();
        
        // then
        Assert.Empty(chatServerRef.RoomTickets);
    }

    [Fact]
    public void CreateChatRoom()
    {
        // given
        var (userTokenSet, _) = chatServerRef.CreateTestUser();

        var createRoomTicket = new ChatServer.CreateRoom
        {
            AccessToken = userTokenSet.Access
        };
        
        chatServerRef.AddTicket(createRoomTicket);
        
        Assert.Empty(chatServerRef.Rooms);
        Assert.Single(chatServerRef.RoomTickets);

        // when
        chatServerRef.CreateRooms();

        // then
        Assert.Single(chatServerRef.Rooms);
        
        var chatRoom = chatServerRef.Rooms.First();
        Assert.True(chatRoom.IsExist());
    }
}


// Helpher
internal static class ChatServerExtensions
{
    internal static (TokenSet, UserInfo) CreateTestUser(this ChatServer chatServerRef)
    { 
        // email & password
        const string testEmail = "TEST_USER_12345@sample.com";
        const string testPassword = "123456";

        var userInfo = new UserInfo
        {
            Email = testEmail,
            Password = testPassword
        };
        
        // create UserCard
        var createUserTicket = new ChatServer.CreateUser
        {
            Email = testEmail,
            Password = testPassword
        };
        
        chatServerRef.AddTicket(createUserTicket);
        chatServerRef.CreateUsers();
        
        // return TokenSet
        var tokenSet = chatServerRef.GetAuthToken(testEmail, testPassword);
        var nonNullTokenSet = Assert.NotNull(tokenSet); // TokenSet 반환
        return (nonNullTokenSet, userInfo);
    }
}


internal readonly record struct UserInfo
{
    internal string Email { get; init; }
    internal string Password { get; init; }
}

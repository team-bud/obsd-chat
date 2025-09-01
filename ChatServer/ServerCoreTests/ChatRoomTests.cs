using ServerCore;
using ToolBox;
using Xunit;


namespace ServerCoreTests.ChatRoomtTests;


// Tests
public class CreatMessages
{
    private readonly ChatServer chatServerRef = new();


    [Fact]
    public void RemoveCreateMessageTicket()
    {
        // given
        var chatRoomRef = chatServerRef.CraeteTestRoom();

        Assert.Empty(chatRoomRef.Tickets);

        var createMessageTicket = new ChatRoom.CreateMessage
        {
            Body = "TEST_MESSAGE"
        };

        chatRoomRef.AddTicket(createMessageTicket);
        Assert.Single(chatRoomRef.Tickets);

        // when
        chatRoomRef.CreateMessages();

        // then
        Assert.Empty(chatRoomRef.Tickets);
    }

    [Fact]
    public void AppendMessage()
    {
        // given
        var chatRoomRef = chatServerRef.CraeteTestRoom();

        var createMessageTicket = new ChatRoom.CreateMessage
        {
            Body = "TEST_MESSAGE"
        };

        chatRoomRef.AddTicket(createMessageTicket);

        Assert.Empty(chatRoomRef.Messages);

        // when
        chatRoomRef.CreateMessages();

        // then
        Assert.Single(chatRoomRef.Messages);
    }

    [Fact]
    public void CreateMessage()
    {
        // given
        var chatRoomRef = chatServerRef.CraeteTestRoom();

        var createMessageTicket = new ChatRoom.CreateMessage
        {
            Body = "TEST_MESSAGE"
        };

        chatRoomRef.AddTicket(createMessageTicket);

        Assert.Empty(chatRoomRef.Messages);

        // when
        chatRoomRef.CreateMessages();

        // then
        Assert.Single(chatRoomRef.Messages);

        var message = chatRoomRef.Messages.First();
        Assert.True(message.IsExist());
    }
}

// Helphers
file static class ChatServerExtensions
{
    public static ChatRoom CraeteTestRoom(this ChatServer chatServerRef)
    {
        // email & password
        const string testEmail = "TEST_USER_12345@sample.com";
        const string testPassword = "123456";

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


        // create ChatRoom
        var createRoomTicket = new ChatServer.CreateRoom
        {
            AccessToken = nonNullTokenSet.Access
        };

        chatServerRef.AddTicket(createRoomTicket);

        Assert.Empty(chatServerRef.Rooms);
        Assert.Single(chatServerRef.RoomTickets);

        chatServerRef.CreateRooms();

        // get ChatRoom
        Assert.Single(chatServerRef.Rooms);
        var chatRoom = chatServerRef.Rooms.First();
        Assert.NotNull(chatRoom.Ref());

        return chatRoom.Ref()!;
    }
}

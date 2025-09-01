using Xunit;
using ServerCore;
using ToolBox;
using System.Reflection;

namespace ServerCoreTests.MessageTests;

// Tests
public class RemoveMessage
{
    private readonly ChatServer chatServerRef = new();

    [Fact]
    public void DeleteMessage()
    {
        // givne
        var messageRef = chatServerRef.CreateTestMessage();

        Assert.True(messageRef.Id.IsExist());

        // when
        messageRef.RemoveMessage();

        // then
        Assert.False(messageRef.Id.IsExist());
    }

    [Fact]
    public void removeMessage_ChatRoom()
    {
        // given
        var messageRef = chatServerRef.CreateTestMessage();

        var chatRoomRef = messageRef.Owner.Ref()!;

        Assert.Contains(messageRef.Id, chatRoomRef.Messages);

        // when
        messageRef.RemoveMessage();

        // then
        Assert.DoesNotContain(messageRef.Id, chatRoomRef.Messages);
    }
}


// Helpher
file static class ChatServerExtensions
{
    public static Message CreateTestMessage(this ChatServer chatServerRef)
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

        // create ChatRoom
        Assert.Single(chatServerRef.Rooms);
        var chatRoom = chatServerRef.Rooms.First();
        if (chatRoom.Ref() is not ChatRoom chatRoomRef)
        {
            Assert.Fail("ChatRoom이 생성되지 않았습니다.");
            throw new Exception("");
        }

        // create Message
        var newMessageTicket = new ChatRoom.CreateMessage
        {
            Body = "TEST_MESSAGE"
        };

        chatRoomRef.AddTicket(newMessageTicket);
        chatRoomRef.CreateMessages();

        Assert.Empty(chatRoomRef.Tickets);
        Assert.Single(chatRoomRef.Messages);

        var message = chatRoomRef.Messages.First();
        Assert.True(message.IsExist());

        return message.Ref()!;
    }
}
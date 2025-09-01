using Xunit;
using ServerCore;
using ToolBox.JWT;

namespace ServerCoreTests.UserCardTests;


// Tests
public class SetRefreshToken
{
    // setUp
    private readonly ChatServer chatServerRef = new ();
    
    [Fact]
    public void SetWithUnknownAccessToken()
    {
        // given
        var newTicket = new ChatServer.CreateUser
        {
            Email = "sample@example.com",
            Password = "123456"
        };
        
        chatServerRef.AddTicket(newTicket);
        
        chatServerRef.CreateUsers();
        
        // given
        Assert.Single(chatServerRef.Users);
        var userCard = chatServerRef.Users.First();

        if (userCard.Ref is not UserCard userCardRef)
        {
            Assert.Fail();
            return;
        }
        

        // given
        Assert.Null(userCardRef.RefreshToken);
        
        // when
        userCardRef.SetRefreshToken(
            new AccessToken()
            {
                RawValue = "AnonymousToken",
                Issuer = "UNKNOWN",
                symKey = SymmerticKey.Random()
            }, 
            "SOME_TOKEN");
        
        // then
        Assert.Null(userCardRef.RefreshToken);
    }

    [Fact]
    public void SetWithValidAccessToken()
    {
        // given
        var newTicket = new ChatServer.CreateUser
        {
            Email = "sample@example.com",
            Password = "123456"
        };
        
        chatServerRef.AddTicket(newTicket);
        
        chatServerRef.CreateUsers();

        var optionalTokenSet = chatServerRef.GetAuthToken(
            "sample@example.com",
            "123456");
        if (optionalTokenSet is not TokenSet tokenSet)
        {
            Assert.Fail();
            return;
        }
        
        
        
        // given
        Assert.Single(chatServerRef.Users);
        var userCard = chatServerRef.Users.First();

        if (userCard.Ref is not UserCard userCardRef)
        {
            Assert.Fail();
            return;
        }
        
        Assert.Null(userCardRef.RefreshToken);
     
        
        // when
        userCardRef.SetRefreshToken(tokenSet.Access, "SOME_TOKEN");
        
        // then
        Assert.Equal("SOME_TOKEN", userCardRef.RefreshToken);
    }
}


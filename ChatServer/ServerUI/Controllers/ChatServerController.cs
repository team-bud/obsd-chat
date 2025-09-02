using Microsoft.AspNetCore.Mvc;
using ServerCore;
using ToolBox.JWT;

namespace ServerUI.Controllers;


// Controller
[ApiController]
[Route("[controller]")]
public class ChatServerController(ChatServer chatServer) : ControllerBase
{
    // JSON Body: { "email": "test@test.com", "password": "1234" }
    [HttpPost("create-user")]
    public TokenSet? CreateUser([FromBody] ChatServer.CreateUser ticket)
    {
        chatServer.AddTicket(ticket);
        chatServer.CreateUsers(); // 티켓 처리

        var tokenSet = chatServer.GetAuthToken(ticket.Email, ticket.Password);
        return tokenSet;
    }

    // JSON Body: { "accessToken": { "rawValue": "..." } }
    [HttpPost("create-room")]
    public IActionResult CreateRoom([FromBody] ChatServer.CreateRoom ticket)
    {
        chatServer.AddTicket(ticket);
        chatServer.CreateRooms(); // 티켓 처리
        
        return Ok(new { message = "Room created", ticket.Id, roomCount = chatServer.Rooms.Count });
    }


    [HttpPost("get-my-rooms")]
    public List<ChatRoom.ID>? GetMyRooms([FromBody] AccessToken accessToken)
    {
        var chatRooms = chatServer.GetMyRooms(accessToken);
        return chatRooms;
    }
}
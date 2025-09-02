using Microsoft.AspNetCore.Mvc;
using ServerCore;

namespace ServerUI.Controllers;


[ApiController]
[Route("[controller]")]
public class ChatRoomController : ControllerBase
{
    [HttpPost("create-message/{id}")]
    public IActionResult CreateMessage(
        Guid id,
        [FromBody] ChatRoom.CreateMessage ticket)
    {
        // ChatRoom.ID 생성
        var chatRoom = ChatRoom.ID.With(id);

        if (chatRoom.Ref() is null)
        {
            Console.WriteLine("Chat room ref is null - CreateMessage");
        }

        chatRoom.Ref()?.AddTicket(ticket);
        chatRoom.Ref()?.CreateMessages();

        return Ok(new { message = "message created", chatRoom.Ref()?.Messages.Count });
    }

    [HttpGet("get-all-messages/{id}")]
    public List<Message.ID>? GetAllMessages(Guid id)
    {
        // ChatRoom.ID 정의
        var chatRoom = ChatRoom.ID.With(id);

        if (chatRoom.Ref() is null)
        {
            Console.WriteLine("Chat room ref is null - GetAllMessages");
        }

        // return Messages
        var messages = chatRoom.Ref()?.Messages;
        return messages;
    }
}
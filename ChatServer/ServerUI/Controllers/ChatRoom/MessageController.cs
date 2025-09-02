using Microsoft.AspNetCore.Mvc;
using ServerCore;

namespace ServerUI.Controllers;


[ApiController]
[Route("[controller]")]
public class MessageController: ControllerBase
{
    [HttpPost("set-body/{id}")]
    public IActionResult SetBody(Guid id, [FromBody] string newBody)
    {
        // Message.ID 생성
        // Message가 존재하지 않는다면 콘솔 출력
        var message = Message.ID.With(id);

        if (message.Ref() is null)
        {
            Console.WriteLine("MessageController.setBody: message is null");
            return Ok();
        }
        
        // Message.Body 수정
        message.Ref()?.Body = newBody;
        
        // 응답
        return Ok();
    }


    [HttpGet("get-body/{id}")]
    public string? GetBody(Guid id)
    {
        // Message.ID 정의
        // Message가 존재하지 않는다면 null
        var message = Message.ID.With(id);

        if (message.Ref() is null)
        {
            Console.WriteLine("MessageController.getBody: message is null");
            return null;
        }

        
        // Message.Body 반환
        var body = message.Ref()?.Body;
        return body;
    }
    
}
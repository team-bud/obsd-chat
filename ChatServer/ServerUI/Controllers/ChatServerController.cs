using Microsoft.AspNetCore.Mvc;
using ServerCore;
using ToolBox;

namespace ServerUI.Controllers;


// Controller
[ApiController]
[Route("[controller]")]
public class ChatServerController: ControllerBase
{
    [HttpGet]
    public int GetSampleNumber()
    {
        return 12345;
    }

    [HttpGet("{id}")]
    public string GetSampleNumber(int id)
    {
        return $"item is {id}!!";
    }
}
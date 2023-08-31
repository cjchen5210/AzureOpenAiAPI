using GPTBackEnd.Data;
using GPTBackEnd.Model;
using Microsoft.AspNetCore.Mvc;

namespace GPTBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateController : ControllerBase
    {
        [HttpGet("{userID}, {content}")]
        public IActionResult Get(string userID, string content)
        {
            try
            {
                using (var dbChatContext = new ChatContext())
                {
                    var userid = new Guid(userID);
                    var Chat = dbChatContext.Chats.FirstOrDefault(u => u.Id == userid);
                    if (Chat != null)
                    {
                        var message = Chat.Message;
                        Dictionary<string, string> question = new Dictionary<string, string>()
                    {
                        {"role", "user" },
                        {"content", content }
                    };
                        message.Add(question);
                        dbChatContext.Chats.Update(Chat);
                        dbChatContext.SaveChanges();
                        return Ok(Chat);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

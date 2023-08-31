using GPTBackEnd.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace GPTBackEnd.Controllers
{
    [Route("api/[controller]/[action]/{chatId}")]
    [ApiController]
    public class GetChatController : Controller
    {
        [HttpGet]
        public IActionResult GetChat(string chatId)
        {
            try
            {
                using (var dbContext = new ChatContext())
                {
                    var id = new Guid(chatId);
                    var Chat = dbContext.Chats.FirstOrDefault(u => u.Id == id);
                    if (Chat == null) return NotFound();
                    var messages = Chat.Message;
                    if (messages == null) return BadRequest();
                    return Ok(Chat);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}

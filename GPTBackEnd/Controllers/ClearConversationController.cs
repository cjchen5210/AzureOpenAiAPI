using GPTBackEnd.Data;
using GPTBackEnd.Model;
using Microsoft.AspNetCore.Mvc;

namespace GPTBackEnd.Controllers
{
    [Route("api/[controller]/[action]/{chatId}")]
    [ApiController]
    public class ClearConversationController : Controller
    {
        [HttpGet]
        public IActionResult Clear(string chatId)
        {
            try
            {
                var chatID = new Guid(chatId);
                using (var dbContext = new ChatContext())
                {
                    var chatDB = dbContext.Chats.FirstOrDefault(u => u.Id == chatID);
                    if (chatDB == null) return BadRequest();
                    Dictionary<string, string> message = new Dictionary<string, string>()
                {
                    {"role", "system" },
                    {"content", "You are an AI assistant that helps people find information." }
                };
                    List<Dictionary<string, string>> messages = new List<Dictionary<string, string>> { message };
                    chatDB.Message = messages;
                    dbContext.Chats.Update(chatDB);
                    dbContext.SaveChanges();
                    return Ok("Clear Conversation Successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

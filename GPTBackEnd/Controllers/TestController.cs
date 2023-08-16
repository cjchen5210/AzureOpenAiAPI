using GPTBackEnd.Data;
using GPTBackEnd.Model;
using Microsoft.AspNetCore.Mvc;

namespace GPTBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet(Name = "GetTest")]
        public string Get()
        {
            Dictionary<string, string> item1 = new Dictionary<string, string>()
            {
                {"role","system" },
                {"content", "AI" },
            };
            Dictionary<string, string> item2 = new Dictionary<string, string>()
            {
                {"role","user" },
                {"content", "Hello" },
            };
            List<Dictionary<string, string>> message = new List<Dictionary<string, string>> { item1, item2 };
            
            using (var dbChatContext = new ChatContext()) 
            {
                var chat1 = new Chat()
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    Created = DateTime.Now,
                    title = "title",
                };
                dbChatContext.Database.EnsureCreated();
                dbChatContext.Chats.Add(chat1);
                dbChatContext.SaveChanges();
                Console.WriteLine("Save to DB");
            } 
            return "API~";
        }
    }
}

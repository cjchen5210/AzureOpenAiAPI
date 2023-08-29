using ChatGPT_WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GPTBackEnd.Model;
using Azure.AI.OpenAI;
using GPTBackEnd.Data;
using Microsoft.Azure.Cosmos;
using TiktokenSharp;

namespace GPTBackEnd.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetConversation([FromBody] Chat chat)
        {
            try
            {
                var messages = chat.Message;
                // convert input data to azure openai standard data
                if (messages == null) return BadRequest();
                List<ChatMessage> chatMessages = new List<ChatMessage>();
                foreach (var message in messages)
                {
                    ChatMessage chatMessage = new ChatMessage(new ChatRole(message["role"]), message["content"]);
                    chatMessages.Add(chatMessage);
                }

                // summary title 
                if (chat.title == null)
                {
                    chat.title = "GPT generated";
                }

                // get response
                var chatCompletion = new ChatCompletion();
                var answer = chatCompletion.GetChatCompletionAsync(chatMessages);
                chatMessages.Add(new ChatMessage(ChatRole.Assistant, answer));

                // query data and compare with database
                var chatID = chat.Id;
                using (var dbContext = new ChatContext())
                {
                    var chatDB = dbContext.Chats.FirstOrDefault(u => u.Id == chatID);
                    if (chatDB == null) return BadRequest();
                    var dbMessage = chatDB.Message;
                    var lastDb = dbMessage.LastOrDefault();
                    var lastChat = chatMessages.LastOrDefault();
                    if (lastChat != null && lastDb != null && (lastChat.Content != lastDb["content"]))
                    {
                        var question = messages.LastOrDefault();

                        var questionDB = new Dictionary<string, string>()
                    {
                        { "role", "user"},
                        { "content", question["content"] }
                    };
                        var answerDB = new Dictionary<string, string>()
                    {
                        { "role", "assistant"},
                        { "content", answer }
                    };
                        dbMessage.Add(questionDB);
                        dbMessage.Add(answerDB);
                        dbContext.Chats.Update(chatDB);
                        dbContext.SaveChanges();
                        return Ok(chatDB);
                    }

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            
        }

        private ChatRole CovertChatRole(string role)
        {
            if (role == "assistant")
            {
                return ChatRole.Assistant;
            }
            else if (role == "system")
            {
                return ChatRole.System;
            }
            else if (role == "user")
            {
                return ChatRole.User;
            }
            return null;
        }

        public static int GetTokenAmount(List<Dictionary<string, string>> messages)
        {
            TikToken tikToken = TikToken.GetEncoding("cl100k_base");
            int totalTokenCount = 0;
            foreach (var message in messages)
            {
                string content = message["content"];
                var encodedTokens = tikToken.Encode(content);
                int tokenCount = encodedTokens.Count();
                totalTokenCount += tokenCount;
            }
            
            return totalTokenCount;
        }
    }
}

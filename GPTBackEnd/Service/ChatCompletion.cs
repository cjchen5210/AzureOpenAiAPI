using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;



namespace ChatGPT_WebAPI.Services
{
    public class ChatCompletion
    {
        private readonly OpenAIClient _client;

        public ChatCompletion()
        {
            //使用ConfigurationBuilder建立配置，从appsettings.json中读取配置数据。
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            // 从配置中提取AZURE_OPENAI_API_KEY。如果没有找到该键，将引发异常。
            // var azureOpenAIApiKey = configuration["AppSettings:AZURE_OPENAI_API_KEY"] ?? throw new InvalidOperationException("The AZURE_OPENAI_API_KEY setting is missing from appsettings.json.");

            // 使用AzureKeyCredential实例化OpenAIClient。
            _client = new OpenAIClient(
                new Uri("https://aoai0602.openai.azure.com/"),
                new AzureKeyCredential("df9789b695e34818933f0aa36562687f")
            );
        }

        // 与 Azure OpenAI API 交互并获取 ChatCompletions
        // 一直维护一个中间的列表，先从数据库获取，然后从得到response，再加入列表，存入数据库
        public string GetChatCompletionAsync(List<ChatMessage> chatMessages)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Temperature = (float)0.7,
                MaxTokens = 6000,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };
            foreach(var chatMessage in chatMessages)
            {
                chatCompletionsOptions.Messages.Add(chatMessage);
            }

            /*Response<StreamingChatCompletions> response = await _client.GetChatCompletionsStreamingAsync(
                "gpt-35-turbo", chatOption
            );*/
            Response<ChatCompletions> response = _client.GetChatCompletions(
            deploymentOrModelName: "gpt-35-turbo",
            chatCompletionsOptions);
            var content = response.Value.Choices[0].Message.Content;
            if (content == null) return "";
            return content;

        }
    }
}
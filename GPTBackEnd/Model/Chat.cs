namespace GPTBackEnd.Model
{
    public class Chat
    {
        public List<Dictionary<string, string>>? Message { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime? Created { get; set; }

        public string? title { get; set; }
    }
}

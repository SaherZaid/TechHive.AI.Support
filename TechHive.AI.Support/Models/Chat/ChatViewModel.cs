namespace TechHive.AI.Support.Models.Chat;

public class ChatViewModel
{
    public List<ChatMessage> Messages { get; set; } = new();
    public string? UserMessage { get; set; }
}

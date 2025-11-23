namespace TechHive.AI.Support.Services;

public interface IAiChatService
{
    Task<string> GetSupportReplyAsync(string userMessage, CancellationToken cancellationToken = default);
}

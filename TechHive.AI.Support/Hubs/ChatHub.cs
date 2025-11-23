using Microsoft.AspNetCore.SignalR;
using TechHive.AI.Support.Services;

namespace TechHive.AI.Support.Hubs;

public class ChatHub : Hub
{
    private readonly IAiChatService _aiChatService;

    public ChatHub(IAiChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    // هذي الميثود بيناديها الجافاسكربت من المتصفح
    public async Task SendMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        // نرسل رسالة اليوزر له نفسه (اختياري)
        await Clients.Caller.SendAsync("ReceiveMessage", "You", message, true);

        // نجيب رد الـ AI
        var reply = await _aiChatService.GetSupportReplyAsync(message);

        // نرسل رد البوت لنفس الشخص
        await Clients.Caller.SendAsync("ReceiveMessage", "TechHive Support", reply, false);
    }
}
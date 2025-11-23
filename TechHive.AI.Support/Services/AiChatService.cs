using System.Text;
using System.Text.Json;
using global::TechHive.AI.Support.Data;
using Microsoft.EntityFrameworkCore;
using TechHive.AI.Support.Data;

namespace TechHive.AI.Support.Services;

public class AiChatService : IAiChatService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _db;

    public AiChatService(HttpClient httpClient, IConfiguration config, ApplicationDbContext db)
    {
        _httpClient = httpClient;
        _config = config;
        _db = db;
    }

    public async Task<string> GetSupportReplyAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        // Load data from DB
        var products = await _db.Products.AsNoTracking().ToListAsync(cancellationToken);
        var faqs = await _db.FaqItems.AsNoTracking().ToListAsync(cancellationToken);
        var settings = await _db.StoreSettings.AsNoTracking().ToListAsync(cancellationToken);

        var storeName = settings.FirstOrDefault(s => s.Key == "StoreName")?.Value ?? "TechHive";

        // Build context
        var contextBuilder = new StringBuilder();
        contextBuilder.AppendLine($"Store name: {storeName}");
        contextBuilder.AppendLine("Products:");
        foreach (var p in products)
        {
            contextBuilder.AppendLine($"- {p.Name}: {p.Description}, Price: {p.Price}, Stock: {p.Stock}");
        }

        contextBuilder.AppendLine();
        contextBuilder.AppendLine("FAQs:");
        foreach (var f in faqs)
        {
            contextBuilder.AppendLine($"Q: {f.Question}");
            contextBuilder.AppendLine($"A: {f.Answer}");
        }

        var context = contextBuilder.ToString();

        var systemPrompt =
    "You are a helpful customer support assistant for an online electronics store based in Sweden called TechHive. " +
    "Always assume the customer is located in Sweden unless they specify another country. " +
    "If the customer asks about shipping, returns, or delivery, respond with information relevant to Sweden or the EU. " +
    "If the question is about another country and no exact data exists, ask for a postal code or inform them that international times may vary. " +
    "Answer in English or Swedish depending on the questioner's language. in a friendly and concise tone. " +
    "If no information exists in the database for that question, politely explain that and recommend contacting human support.\n\n" +
    context;

        var requestBody = new
        {
            model = "gpt-4.1-mini", // example
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            }
        };

        var apiKey = _config["OpenAI:ApiKey"]; // put this in user secrets / env
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var json = JsonSerializer.Serialize(requestBody);
        var response = await _httpClient.PostAsync(
            "https://api.openai.com/v1/chat/completions",
            new StringContent(json, Encoding.UTF8, "application/json"),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(responseJson);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "Sorry, I could not generate a reply.";

        return content;
    }
}

using System.Text.Json;

namespace SSEDemo;

public static class SSEHttpContextExtensions 
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
	
    public static async Task SSEInitAsync(this HttpResponse response)
    {
        response.Headers.Add("Content-Type", "text/event-stream");
        await response.Body.FlushAsync();
    }
	
    public static async Task SSESendDataAsync(this HttpResponse response, string data)
    {
        foreach(var line in data.Split('\n'))
            await response.WriteAsync("data: " + line + "\n");
        
        await response.WriteAsync("\n");
        await response.Body.FlushAsync();
    }
	
    public static async Task SSESendEventAsync(this HttpResponse response, ISSEEvent e, string? sseEventName = null)
    {
        await response.WriteAsync($"event: {sseEventName ?? e.Name}\n");

        var lines = e.Payload switch
        {
            null        => new [] { string.Empty },
            string s    => s.Split('\n').ToArray(),
            Exception x => new [] { JsonSerializer.Serialize(new { Type = "Exception", x.Message, x.StackTrace }, Options) },
            _           => new [] { JsonSerializer.Serialize(e.Payload, Options) }
        };

        foreach(var line in lines)
            await response.WriteAsync($"data: {line}\n");

        await response.WriteAsync("\n");
        await response.Body.FlushAsync();
    }
	
    public static async Task SSESendCloseAsync(this HttpResponse response)
    {
        await response.SSESendEventAsync(new SSEEvent("close", null));
    } 
}
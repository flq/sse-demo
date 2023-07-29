using SSEDemo;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGet("/api/events", () => SSEResult.From(ProduceEvents()));

app.Run();
return;

static async IAsyncEnumerable<SSEEvent> ProduceEvents()
{
    var r = new Random();
    foreach (var _ in Enumerable.Range(0, 10))
    {
        var value = r.Next(300, 1000);
        await Task.Delay(value);
        yield return new SSEEvent("Timestamp", new { Value = value });
    }
}
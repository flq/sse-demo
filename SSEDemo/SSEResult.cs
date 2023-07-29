namespace SSEDemo;

public static class SSEResult
{
    public static IResult From<T>(IAsyncEnumerable<T> producer) where T : ISSEEvent
    {
        return new Implementation<T>(producer);
    }

    private class Implementation<T> : IResult where T : ISSEEvent
    {
        private readonly IAsyncEnumerable<T> producer;

        public Implementation(IAsyncEnumerable<T> producer)
        {
            this.producer = producer;
        }

        async Task IResult.ExecuteAsync(HttpContext httpContext)
        {
            var response = httpContext.Response;
            await response.SSEInitAsync();
            await foreach (var @event in producer)
            {
                await response.SSESendEventAsync(@event);
            }

            await response.SSESendCloseAsync();
            await response.CompleteAsync();
        }
    }
}

public interface ISSEEvent
{
    string Name { get; }
    object? Payload { get; }
}
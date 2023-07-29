namespace SSEDemo;

public record struct SSEEvent(string Name, object? Payload) : ISSEEvent;
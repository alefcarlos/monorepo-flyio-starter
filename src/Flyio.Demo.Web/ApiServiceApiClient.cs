using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flyio.Demo.Web;

public class ApiServiceApiClient(HttpClient httpClient)
{
    public async Task<GetAllTodosResponse[]> GetAllTodosAsync()
    {
        List<GetAllTodosResponse>? forecasts = null;

        await foreach (var data in httpClient.GetFromJsonAsAsyncEnumerable<GetAllTodosResponse>("/v1/todos"))
        {
            forecasts ??= [];
            forecasts.Add(data!);
        }

        return forecasts?.ToArray() ?? [];
    }

    public async Task PostTodosAsync(PostTodoRequest request)
    {
        await httpClient.PostAsJsonAsync("/v1/todos", request);
    }

    public async IAsyncEnumerable<HeartRateRecord?> SubscribeHeartRateAsync(
        [EnumeratorCancellation]CancellationToken cancellationToken)
    {
        using var stream = await httpClient.GetStreamAsync("/heart-rate", cancellationToken);
        await foreach (SseItem<HeartRateRecord?> item in SseParser.Create(stream, (eventType, bytes) =>
            JsonSerializer.Deserialize<HeartRateRecord>(bytes)).EnumerateAsync(cancellationToken))
        {
            yield return item.Data;
        }
    }
}

public record GetAllTodosResponse(Guid Id, string Name);
public record PostTodoRequest(string Name);

public record HeartRateRecord(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("heartRate")] int HeartRate
);
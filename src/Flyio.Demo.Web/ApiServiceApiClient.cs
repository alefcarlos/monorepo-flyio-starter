using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flyio.Demo.Web;

public class ApiServiceApiClient(HttpClient httpClient)
{
    public async Task<GetAllTodosResponse[]> GetAllTodosAsync(string? search)
    {
        List<GetAllTodosResponse>? forecasts = null;

        await foreach (var data in httpClient.GetFromJsonAsAsyncEnumerable<GetAllTodosResponse>("/v1/todos"))
        {
            forecasts ??= [];
            forecasts.Add(data!);
        }

        var allData = forecasts?.ToArray() ?? [];

        return string.IsNullOrWhiteSpace(search) ? allData : [.. allData.Where(x => x.Name.Contains(search))];
    }

    public async Task PostTodosAsync(PostTodoRequest request)
    {
        await httpClient.PostAsJsonAsync("/v1/todos", request);
    }

    public async IAsyncEnumerable<HeartRateRecord?> SubscribeHeartRateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var stream = await httpClient.GetStreamAsync("/heart-rate", cancellationToken);
        await foreach (SseItem<HeartRateRecord?> item in SseParser.Create(stream, (eventType, bytes) =>
            JsonSerializer.Deserialize<HeartRateRecord>(bytes)).EnumerateAsync(cancellationToken))
        {
            yield return item.Data;
        }
    }
}

public record GetAllTodosResponse(Guid Id, string Name, DateTimeOffset CreatedAt);
public record PostTodoRequest(string Name);

public record HeartRateRecord(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("heartRate")] int HeartRate
);
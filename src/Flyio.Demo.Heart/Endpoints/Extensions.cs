using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing;

public static class Extensions
{
    public static IEndpointRouteBuilder MapHeartEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/heart-rate", (CancellationToken cancellationToken) =>
        {
            static async IAsyncEnumerable<HeartRateRecord> GetDataAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var rate = Random.Shared.Next(69, 120);
                    yield return HeartRateRecord.Create(rate);
                    await Task.Delay(1_000, cancellationToken);
                }
            }

            return TypedResults.ServerSentEvents(GetDataAsync(cancellationToken), eventType: "heartRate");
        });

        return endpoints;
    }
}

record HeartRateRecord(DateTimeOffset Timestamp, int HeartRate)
{
    public static HeartRateRecord Create(int heartRate) => new(DateTimeOffset.Now, heartRate);
}
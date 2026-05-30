using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Mediator;

namespace Flyio.Demo.SharedKernel;

public static class ModuleMetadata
{
  private static readonly ConcurrentDictionary<Assembly, string> Cache = new();

  public static string GetModuleName(Assembly assembly)
  {
    return Cache.GetOrAdd(assembly, static a =>
    {
      var moduleName = a
              .GetCustomAttributes<AssemblyMetadataAttribute>()
              .FirstOrDefault(x => x.Key == "ModuleName")
              ?.Value;

      return !string.IsNullOrWhiteSpace(moduleName)
              ? moduleName
              : a.GetName().Name ?? "Unknown";
    });
  }
}

public abstract class NotificationHandlerBase<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
  public async ValueTask Handle(TNotification notification, CancellationToken cancellationToken)
  {
    var moduleName = ModuleMetadata.GetModuleName(GetType().Assembly);

    Activity.Current?.SetTag("module.name", moduleName);

    await HandleInternalAsync(notification, cancellationToken);
  }

  protected abstract ValueTask HandleInternalAsync(TNotification notification, CancellationToken cancellationToken);
}

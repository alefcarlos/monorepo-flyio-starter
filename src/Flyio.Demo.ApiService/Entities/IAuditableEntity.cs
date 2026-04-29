//https://github.com/PlatformPlatform/platformplatform/tree/main/shared-kernel
namespace Flyio.Demo.ApiService.Entities;

public interface IAuditableEntity
{
    public string CreatedBy { get; }
    public string? ModifiedBy { get; }
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset? ModifiedAt { get; }

    void UpdateModifiedAt(DateTimeOffset? modifiedAt);
    void UpdateCreatedBy(string userId);
    void UpdateModified(string userId, DateTimeOffset? modifiedAt);
}

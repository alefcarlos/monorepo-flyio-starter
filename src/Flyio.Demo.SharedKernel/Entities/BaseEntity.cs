//https://github.com/PlatformPlatform/platformplatform/tree/main/shared-kernel
using System.ComponentModel.DataAnnotations;

namespace Flyio.Demo.SharedKernel.Entities;

public abstract class BaseEntity : IAuditableEntity
{
    protected BaseEntity()
    {
        CreatedAt = DateTimeOffset.Now;
    }

    public string CreatedBy { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; internal set; }

    [ConcurrencyCheck]
    public string? ModifiedBy { get; private set; }

    [ConcurrencyCheck]
    public DateTimeOffset? ModifiedAt { get; private set; }


    /// <summary>
    ///     This method is used by the UpdateAuditableEntitiesInterceptor in the Infrastructure layer.
    ///     It's not intended to be used by the application, which is why it is implemented using an explicit interface.
    /// </summary>
    void IAuditableEntity.UpdateModifiedAt(DateTimeOffset? modifiedAt)
    {
        ModifiedAt = modifiedAt;
    }

    /// <summary>
    ///     This method is used by the UpdateAuditableEntitiesInterceptor in the Infrastructure layer.
    ///     It's not intended to be used by the application, which is why it is implemented using an explicit interface.
    /// </summary>
    void IAuditableEntity.UpdateCreatedBy(string userId)
    {
        CreatedBy = userId;
    }

    /// <summary>
    ///     This method is used by the UpdateAuditableEntitiesInterceptor in the Infrastructure layer.
    ///     It's not intended to be used by the application, which is why it is implemented using an explicit interface.
    /// </summary>
    void IAuditableEntity.UpdateModified(string userId, DateTimeOffset? modifiedAt)
    {
        ModifiedBy = userId;
        ModifiedAt = modifiedAt;
    }
}
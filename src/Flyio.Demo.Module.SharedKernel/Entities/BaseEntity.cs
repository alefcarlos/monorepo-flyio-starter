//https://github.com/PlatformPlatform/platformplatform/tree/main/shared-kernel
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Flyio.Demo.SharedKernel;

namespace Flyio.Demo.Module.SharedKernel.Entities;

public abstract class BaseEntity : IAuditableEntity, IHaveDomainEvents
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

    private List<DomainEventBase> _domainEvents = new();
    
    [NotMapped]
    public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
    void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();

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

    public void ClearDomainEvents()
    {
        throw new NotImplementedException();
    }
}
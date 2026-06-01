namespace Flyio.Demo.Module.SharedKernel.Entities;

public enum ETrailType
{
    Create,
    Edit,
    Delete,
    Undefined
}

public class AuditTrailEntity
{
    public Guid Id { get; set; }
    public required string EntityName { get; set; }
    public ETrailType TrailType { get; set; }
    public required string UserIdentifier { get; set; }
    public required string OldValues { get; set; }
    public required string NewValues { get; set; }
    public required string AffectedColumns { get; set; }
    public required string PrimaryKey { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
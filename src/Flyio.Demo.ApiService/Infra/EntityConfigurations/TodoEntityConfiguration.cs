using Flyio.Demo.ApiService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flyio.Demo.ApiService.Infra.EntityConfigurations;

public class TodoEntityConfiguration : IEntityTypeConfiguration<TodoEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TodoEntity> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new(value))
            ;
    }
}
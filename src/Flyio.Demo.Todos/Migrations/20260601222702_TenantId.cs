using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flyio.Demo.Todos.Migrations
{
    /// <inheritdoc />
    public partial class TenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Todos",
                table: "Todos",
                type: "text",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Todos",
                table: "Todos");
        }
    }
}

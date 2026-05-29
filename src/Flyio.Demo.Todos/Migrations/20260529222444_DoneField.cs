using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flyio.Demo.Todos.Migrations
{
    /// <inheritdoc />
    public partial class DoneField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Done",
                schema: "Todos",
                table: "Todos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Done",
                schema: "Todos",
                table: "Todos");
        }
    }
}

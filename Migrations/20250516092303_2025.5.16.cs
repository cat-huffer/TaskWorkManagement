using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskWorkManagement.Migrations
{
    /// <inheritdoc />
    public partial class _2025516 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Work");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Work");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Member");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Work",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Work",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Member",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

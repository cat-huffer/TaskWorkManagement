using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskWorkManagement.Migrations
{
    /// <inheritdoc />
    public partial class _20250528 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Director",
                table: "MemberWork");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "MemberWork",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

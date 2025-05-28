using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskWorkManagement.Migrations
{
    /// <inheritdoc />
    public partial class sdsfs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "Work");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Work");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "MemberWork");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "MemberWork",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "MemberWork",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "MemberWork");

            migrationBuilder.DropColumn(
                name: "Director",
                table: "MemberWork");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "Work",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Work",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "MemberWork",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUrlToTaskFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "TaskFiles");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "TaskFiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "TaskFiles");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "TaskFiles",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}

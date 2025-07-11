using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracking.Migrations
{
    /// <inheritdoc />
    public partial class InitialCre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_UserWorkOutTask_UserWorkOutTaskId",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_UserWorkOutTaskId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "UserWorkOutTaskId",
                table: "Workouts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserWorkOutTaskId",
                table: "Workouts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserWorkOutTaskId",
                table: "Workouts",
                column: "UserWorkOutTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_UserWorkOutTask_UserWorkOutTaskId",
                table: "Workouts",
                column: "UserWorkOutTaskId",
                principalTable: "UserWorkOutTask",
                principalColumn: "Id");
        }
    }
}

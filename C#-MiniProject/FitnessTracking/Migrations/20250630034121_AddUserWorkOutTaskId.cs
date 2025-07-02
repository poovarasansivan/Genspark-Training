using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracking.Migrations
{
    /// <inheritdoc />
    public partial class AddUserWorkOutTaskId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkOutPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOutPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoachClientMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachClientMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachClientMaps_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoachClientMaps_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgressUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkOutPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: false),
                    BodyFatPercentage = table.Column<double>(type: "double precision", nullable: false),
                    MuscleMass = table.Column<double>(type: "double precision", nullable: false),
                    WaterPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressUpdates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgressUpdates_WorkOutPlans_WorkOutPlanId",
                        column: x => x.WorkOutPlanId,
                        principalTable: "WorkOutPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkOutTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    Sets = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkOutTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWorkOutTask_Users_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserWorkOutTask_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserWorkOutTask_WorkOutPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "WorkOutPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgressId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageData = table.Column<byte[]>(type: "bytea", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressImage_ProgressUpdates_ProgressId",
                        column: x => x.ProgressId,
                        principalTable: "ProgressUpdates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkOutPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkOutPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCompleted = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserWorkOutTaskId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkOutPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWorkOutPlans_UserWorkOutTask_UserWorkOutTaskId",
                        column: x => x.UserWorkOutTaskId,
                        principalTable: "UserWorkOutTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWorkOutPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkOutPlans_WorkOutPlans_WorkOutPlanId",
                        column: x => x.WorkOutPlanId,
                        principalTable: "WorkOutPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkOutPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CaloriesBurned = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserWorkOutTaskId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workouts_UserWorkOutTask_UserWorkOutTaskId",
                        column: x => x.UserWorkOutTaskId,
                        principalTable: "UserWorkOutTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workouts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workouts_WorkOutPlans_WorkOutPlanId",
                        column: x => x.WorkOutPlanId,
                        principalTable: "WorkOutPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoachClientMaps_ClientId",
                table: "CoachClientMaps",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachClientMaps_CoachId_ClientId",
                table: "CoachClientMaps",
                columns: new[] { "CoachId", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressImage_ProgressId",
                table: "ProgressImage",
                column: "ProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdates_UserId",
                table: "ProgressUpdates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdates_WorkOutPlanId",
                table: "ProgressUpdates",
                column: "WorkOutPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkOutPlans_UserId",
                table: "UserWorkOutPlans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkOutPlans_UserWorkOutTaskId",
                table: "UserWorkOutPlans",
                column: "UserWorkOutTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkOutPlans_WorkOutPlanId",
                table: "UserWorkOutPlans",
                column: "WorkOutPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkOutTask_CoachId",
                table: "UserWorkOutTask",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkOutTask_PlanId",
                table: "UserWorkOutTask",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkOutTask_UserId",
                table: "UserWorkOutTask",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId",
                table: "Workouts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserWorkOutTaskId",
                table: "Workouts",
                column: "UserWorkOutTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_WorkOutPlanId",
                table: "Workouts",
                column: "WorkOutPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoachClientMaps");

            migrationBuilder.DropTable(
                name: "ProgressImage");

            migrationBuilder.DropTable(
                name: "UserWorkOutPlans");

            migrationBuilder.DropTable(
                name: "Workouts");

            migrationBuilder.DropTable(
                name: "ProgressUpdates");

            migrationBuilder.DropTable(
                name: "UserWorkOutTask");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkOutPlans");
        }
    }
}

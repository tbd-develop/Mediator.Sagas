using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TbdDevelop.Mediator.Sagas.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sagas");

            migrationBuilder.CreateTable(
                name: "Sagas",
                schema: "sagas",
                columns: table => new
                {
                    OrchestrationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeIdentifier = table.Column<string>(type: "text", nullable: false),
                    IsComplete = table.Column<bool>(type: "boolean", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    NextTriggerTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TriggerInterval = table.Column<TimeSpan>(type: "interval", nullable: true),
                    LastTriggered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saga_orchestration_id", x => x.OrchestrationIdentifier);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sagas",
                schema: "sagas");
        }
    }
}

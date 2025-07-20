using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TbdDevelop.Mediator.Sagas.SqlServer.Migrations
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
                    OrchestrationIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextTriggerTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    TriggerInterval = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastTriggered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaximumTriggerCount = table.Column<int>(type: "int", nullable: false)
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

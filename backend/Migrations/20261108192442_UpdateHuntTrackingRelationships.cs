using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltimateShinyDex.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHuntTrackingRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hunts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsesCounter = table.Column<bool>(type: "bit", nullable: false),
                    CounterStep = table.Column<int>(type: "int", nullable: false),
                    CurrentEncounters = table.Column<int>(type: "int", nullable: false),
                    TotalEncounters = table.Column<int>(type: "int", nullable: false),
                    CurrentPhaseNumber = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hunts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HuntGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HuntId = table.Column<int>(type: "int", nullable: false),
                    Game = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShinyOdds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HuntGames_Hunts_HuntId",
                        column: x => x.HuntId,
                        principalTable: "Hunts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HuntPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HuntId = table.Column<int>(type: "int", nullable: false),
                    PhaseNumber = table.Column<int>(type: "int", nullable: false),
                    Encounters = table.Column<int>(type: "int", nullable: true),
                    FoundShinyId = table.Column<int>(type: "int", nullable: true),
                    WasTarget = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HuntPhases_Hunts_HuntId",
                        column: x => x.HuntId,
                        principalTable: "Hunts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HuntPhases_Shinies_FoundShinyId",
                        column: x => x.FoundShinyId,
                        principalTable: "Shinies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HuntTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HuntId = table.Column<int>(type: "int", nullable: false),
                    Pokemon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Form = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAlpha = table.Column<bool>(type: "bit", nullable: true),
                    Mark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HuntTargets_Hunts_HuntId",
                        column: x => x.HuntId,
                        principalTable: "Hunts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HuntEncounters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HuntGameId = table.Column<int>(type: "int", nullable: false),
                    Pokemon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Form = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EncounterPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntEncounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HuntEncounters_HuntGames_HuntGameId",
                        column: x => x.HuntGameId,
                        principalTable: "HuntGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HuntEncounters_HuntGameId",
                table: "HuntEncounters",
                column: "HuntGameId");

            migrationBuilder.CreateIndex(
                name: "IX_HuntGames_HuntId",
                table: "HuntGames",
                column: "HuntId");

            migrationBuilder.CreateIndex(
                name: "IX_HuntPhases_FoundShinyId",
                table: "HuntPhases",
                column: "FoundShinyId",
                unique: true,
                filter: "[FoundShinyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HuntPhases_HuntId",
                table: "HuntPhases",
                column: "HuntId");

            migrationBuilder.CreateIndex(
                name: "IX_HuntTargets_HuntId",
                table: "HuntTargets",
                column: "HuntId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HuntEncounters");

            migrationBuilder.DropTable(
                name: "HuntPhases");

            migrationBuilder.DropTable(
                name: "HuntTargets");

            migrationBuilder.DropTable(
                name: "HuntGames");

            migrationBuilder.DropTable(
                name: "Hunts");
        }
    }
}

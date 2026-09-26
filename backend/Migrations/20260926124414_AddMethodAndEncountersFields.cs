using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltimateShinyDex.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMethodAndEncountersFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Encounters",
                table: "Shinies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "Shinies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Encounters",
                table: "Shinies");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "Shinies");
        }
    }
}

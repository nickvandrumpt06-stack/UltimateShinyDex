using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltimateShinyDex.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAlphaAndMarkFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAlpha",
                table: "Shinies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Mark",
                table: "Shinies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlpha",
                table: "Shinies");

            migrationBuilder.DropColumn(
                name: "Mark",
                table: "Shinies");
        }
    }
}

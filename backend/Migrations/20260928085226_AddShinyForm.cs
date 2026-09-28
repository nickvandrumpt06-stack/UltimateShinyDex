using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltimateShinyDex.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddShinyForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Form",
                table: "Shinies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Form",
                table: "Shinies");
        }
    }
}

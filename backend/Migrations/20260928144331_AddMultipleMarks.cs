using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltimateShinyDex.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleMarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mark",
                table: "Shinies");

            migrationBuilder.CreateTable(
                name: "ShinyMarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShinyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShinyMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShinyMarks_Shinies_ShinyId",
                        column: x => x.ShinyId,
                        principalTable: "Shinies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShinyMarks_ShinyId",
                table: "ShinyMarks",
                column: "ShinyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShinyMarks");

            migrationBuilder.AddColumn<string>(
                name: "Mark",
                table: "Shinies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

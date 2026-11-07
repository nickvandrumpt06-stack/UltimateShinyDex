using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltimateShinyDex.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDisabledDexSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisabledDexSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DexNumber = table.Column<int>(type: "int", nullable: false),
                    Pokemon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Form = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabledDexSlots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisabledDexSlots_DexNumber_Pokemon_Form_Gender",
                table: "DisabledDexSlots",
                columns: new[] { "DexNumber", "Pokemon", "Form", "Gender" },
                unique: true,
                filter: "[Form] IS NOT NULL AND [Gender] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisabledDexSlots");
        }
    }
}

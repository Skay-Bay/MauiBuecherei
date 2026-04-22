using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buecher",
                columns: table => new
                {
                    Buchnummer = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sachgebiet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutorInnen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Verlag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Verlagsort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Erscheinungsdatum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buecher", x => x.Buchnummer);
                });

            migrationBuilder.CreateTable(
                name: "SchülerIn",
                columns: table => new
                {
                    Ausweisnummer = table.Column<int>(type: "int", nullable: false),
                    Vorname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nachname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchülerIn", x => x.Ausweisnummer);
                });

            migrationBuilder.CreateTable(
                name: "Ausleihen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Buchnummer = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ausweisnummer = table.Column<int>(type: "int", nullable: true),
                    Ausleihdatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rueckgabedatum = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ausleihen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ausleihen_Buecher_Buchnummer",
                        column: x => x.Buchnummer,
                        principalTable: "Buecher",
                        principalColumn: "Buchnummer",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ausleihen_SchülerIn_Ausweisnummer",
                        column: x => x.Ausweisnummer,
                        principalTable: "SchülerIn",
                        principalColumn: "Ausweisnummer");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ausleihen_Ausweisnummer",
                table: "Ausleihen",
                column: "Ausweisnummer");

            migrationBuilder.CreateIndex(
                name: "IX_Ausleihen_Buchnummer",
                table: "Ausleihen",
                column: "Buchnummer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ausleihen");

            migrationBuilder.DropTable(
                name: "Buecher");

            migrationBuilder.DropTable(
                name: "SchülerIn");
        }
    }
}

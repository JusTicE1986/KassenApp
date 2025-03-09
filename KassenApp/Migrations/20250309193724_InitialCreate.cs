using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KassenApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Konten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KontoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Konten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buchungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Buchungsart = table.Column<int>(type: "int", nullable: false),
                    Verwendungszweck = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Betrag = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateiPfad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KontoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buchungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buchungen_Konten_KontoId",
                        column: x => x.KontoId,
                        principalTable: "Konten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buchungen_KontoId",
                table: "Buchungen",
                column: "KontoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Buchungen");

            migrationBuilder.DropTable(
                name: "Konten");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiRRHH.Services.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Dni = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Dni);
                });

            migrationBuilder.CreateTable(
                name: "Empleos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Periodo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CandidatoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empleos_Candidatos",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Dni");
                });

            migrationBuilder.InsertData(
                table: "Candidatos",
                columns: new[] { "Dni", "Apellido", "Email", "FechaNacimiento", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 15111444, "PEREZ", "lperez@hotmail.com", new DateTime(1985, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "NICOLAS", "34926694444" },
                    { 18222444, "GOMEZ", "lperez@hotmail.com", new DateTime(1985, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "JUAN", "34926694444" },
                    { 28574087, "HIDALGO", "sebahidalgo@hotmail.com", new DateTime(1981, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "SEBASTIAN", "3492669439" },
                    { 31333444, "PEREZ", "lperez@hotmail.com", new DateTime(1985, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "LORENZO", "34926694444" }
                });

            migrationBuilder.InsertData(
                table: "Empleos",
                columns: new[] { "Id", "CandidatoId", "NombreEmpresa", "Periodo" },
                values: new object[,]
                {
                    { 1, 28574087, "Megatone.Net", "2021-02 2021-06" },
                    { 2, 28574087, "Megatone.Net", "2021-07 2021-12" },
                    { 3, 28574087, "Megatone.Net", "2022-01 2022-06" },
                    { 4, 15111444, "MegaCash", "2021-02 2021-06" },
                    { 5, 15111444, "Megatone.Net", "2021-07 2021-12" },
                    { 6, 18222444, "Megatone.Net", "2022-01 2022-06" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleos_CandidatoId",
                table: "Empleos",
                column: "CandidatoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empleos");

            migrationBuilder.DropTable(
                name: "Candidatos");
        }
    }
}

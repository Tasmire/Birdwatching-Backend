using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project_Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManySpawnLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpawnLocations_Animals_AnimalId",
                table: "SpawnLocations");

            migrationBuilder.DropIndex(
                name: "IX_SpawnLocations_AnimalId",
                table: "SpawnLocations");

            migrationBuilder.DropColumn(
                name: "AnimalId",
                table: "SpawnLocations");

            migrationBuilder.CreateTable(
                name: "AnimalSpawnLocations",
                columns: table => new
                {
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpawnLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalSpawnLocations", x => new { x.AnimalId, x.SpawnLocationId });
                    table.ForeignKey(
                        name: "FK_AnimalSpawnLocations_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "AnimalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalSpawnLocations_SpawnLocations_SpawnLocationId",
                        column: x => x.SpawnLocationId,
                        principalTable: "SpawnLocations",
                        principalColumn: "SpawnLocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalSpawnLocations_SpawnLocationId",
                table: "AnimalSpawnLocations",
                column: "SpawnLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalSpawnLocations");

            migrationBuilder.AddColumn<Guid>(
                name: "AnimalId",
                table: "SpawnLocations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SpawnLocations_AnimalId",
                table: "SpawnLocations",
                column: "AnimalId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpawnLocations_Animals_AnimalId",
                table: "SpawnLocations",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "AnimalId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

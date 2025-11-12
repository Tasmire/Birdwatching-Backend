using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project_Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class SpawnLocationsEnvironmentFKRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpawnLocations_Environments_EnvironmentId",
                table: "SpawnLocations");

            migrationBuilder.DropIndex(
                name: "IX_SpawnLocations_EnvironmentId",
                table: "SpawnLocations");

            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                table: "SpawnLocations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EnvironmentId",
                table: "SpawnLocations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SpawnLocations_EnvironmentId",
                table: "SpawnLocations",
                column: "EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpawnLocations_Environments_EnvironmentId",
                table: "SpawnLocations",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "EnvironmentId");
        }
    }
}

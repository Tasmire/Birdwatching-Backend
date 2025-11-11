using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project_Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSpawnLocationFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpawnLocations_Environments_EnvironmentId",
                table: "SpawnLocations");

            migrationBuilder.AddForeignKey(
                name: "FK_SpawnLocations_Environments_EnvironmentId",
                table: "SpawnLocations",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "EnvironmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpawnLocations_Environments_EnvironmentId",
                table: "SpawnLocations");

            migrationBuilder.AddForeignKey(
                name: "FK_SpawnLocations_Environments_EnvironmentId",
                table: "SpawnLocations",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "EnvironmentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

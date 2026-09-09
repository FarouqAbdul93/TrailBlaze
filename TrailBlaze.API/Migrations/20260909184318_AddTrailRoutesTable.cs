using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace TrailBlaze.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTrailRoutesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // RouteData column already dropped manually in SSMS
            // TrailRoutes table already created manually in SSMS
            // Just create the index EF Core expects
            migrationBuilder.CreateIndex(
                name: "IX_TrailRoutes_TrailId",
                table: "TrailRoutes",
                column: "TrailId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrailRoutes");

            migrationBuilder.AddColumn<string>(
                name: "RouteData",
                table: "Trails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class ZoneServerUniqueEndpointContraintAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZoneServerAddress",
                table: "zone_endpoints",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<bool>(
                name: "IsValidated",
                table: "world_entry",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_zone_endpoints_ZoneServerAddress_ZoneServerPort",
                table: "zone_endpoints",
                columns: new[] { "ZoneServerAddress", "ZoneServerPort" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_zone_endpoints_ZoneServerAddress_ZoneServerPort",
                table: "zone_endpoints");

            migrationBuilder.DropColumn(
                name: "IsValidated",
                table: "world_entry");

            migrationBuilder.AlterColumn<string>(
                name: "ZoneServerAddress",
                table: "zone_endpoints",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}

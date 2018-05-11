using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class AddedZoneTypeToZoneModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isStatic",
                table: "zone_endpoints");

            migrationBuilder.AddColumn<int>(
                name: "ZoneType",
                table: "zone_endpoints",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ZoneType",
                table: "zone_endpoints");

            migrationBuilder.AddColumn<bool>(
                name: "isStatic",
                table: "zone_endpoints",
                nullable: false,
                defaultValue: false);
        }
    }
}

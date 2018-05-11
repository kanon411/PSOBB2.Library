using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class FixedIncorrectForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_zone_endpoints_character_sessions_ZoneId",
                table: "zone_endpoints");

            migrationBuilder.CreateIndex(
                name: "IX_character_sessions_ZoneId",
                table: "character_sessions",
                column: "ZoneId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_character_sessions_zone_endpoints_ZoneId",
                table: "character_sessions",
                column: "ZoneId",
                principalTable: "zone_endpoints",
                principalColumn: "ZoneId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_sessions_zone_endpoints_ZoneId",
                table: "character_sessions");

            migrationBuilder.DropIndex(
                name: "IX_character_sessions_ZoneId",
                table: "character_sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_zone_endpoints_character_sessions_ZoneId",
                table: "zone_endpoints",
                column: "ZoneId",
                principalTable: "character_sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

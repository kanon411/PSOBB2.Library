using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class AddFluentForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_character_sessions_characters_CharacterId",
                table: "character_sessions",
                column: "CharacterId",
                principalTable: "characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_zone_endpoints_character_sessions_ZoneId",
                table: "zone_endpoints",
                column: "ZoneId",
                principalTable: "character_sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_sessions_characters_CharacterId",
                table: "character_sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_zone_endpoints_character_sessions_ZoneId",
                table: "zone_endpoints");
        }
    }
}

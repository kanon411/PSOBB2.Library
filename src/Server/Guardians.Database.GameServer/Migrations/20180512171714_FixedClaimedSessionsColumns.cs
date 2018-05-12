using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class FixedClaimedSessionsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_claimed_sessions_characters_CharacterEntryCharacterId",
                table: "claimed_sessions");

            migrationBuilder.DropIndex(
                name: "IX_claimed_sessions_CharacterEntryCharacterId",
                table: "claimed_sessions");

            migrationBuilder.DropColumn(
                name: "CharacterEntryCharacterId",
                table: "claimed_sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_claimed_sessions_characters_CharacterId",
                table: "claimed_sessions",
                column: "CharacterId",
                principalTable: "characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_claimed_sessions_characters_CharacterId",
                table: "claimed_sessions");

            migrationBuilder.AddColumn<int>(
                name: "CharacterEntryCharacterId",
                table: "claimed_sessions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_claimed_sessions_CharacterEntryCharacterId",
                table: "claimed_sessions",
                column: "CharacterEntryCharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_claimed_sessions_characters_CharacterEntryCharacterId",
                table: "claimed_sessions",
                column: "CharacterEntryCharacterId",
                principalTable: "characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace PSOBB.Database.GameServer.Migrations
{
    public partial class AddedGroupMembershipMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "group_members",
                columns: table => new
                {
                    CharacterId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_members", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_group_members_characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_members_guild_entry_GroupId",
                        column: x => x.GroupId,
                        principalTable: "guild_entry",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_members_GroupId",
                table: "group_members",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_members");
        }
    }
}

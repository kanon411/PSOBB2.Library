using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class CharacterFriendRequestsTableMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_friendrequests",
                columns: table => new
                {
                    FriendshipId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterOneId = table.Column<int>(nullable: false),
                    CharacterTwoId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TIMESTAMP(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_friendrequests", x => x.FriendshipId);
                    table.ForeignKey(
                        name: "FK_character_friendrequests_characters_CharacterOneId",
                        column: x => x.CharacterOneId,
                        principalTable: "characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_friendrequests_characters_CharacterTwoId",
                        column: x => x.CharacterTwoId,
                        principalTable: "characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_friendrequests_CharacterOneId",
                table: "character_friendrequests",
                column: "CharacterOneId");

            migrationBuilder.CreateIndex(
                name: "IX_character_friendrequests_CharacterTwoId",
                table: "character_friendrequests",
                column: "CharacterTwoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_friendrequests");
        }
    }
}

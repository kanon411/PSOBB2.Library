using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class FriendRequestsWithAlternateKeysMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_friendrequests",
                columns: table => new
                {
                    FriendshipRequestId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RequestingCharacterId = table.Column<int>(nullable: false),
                    TargetRequestCharacterId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TIMESTAMP(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_friendrequests", x => x.FriendshipRequestId);
                    table.UniqueConstraint("AK_character_friendrequests_RequestingCharacterId_TargetRequest~", x => new { x.RequestingCharacterId, x.TargetRequestCharacterId });
                    table.UniqueConstraint("AK_character_friendrequests_TargetRequestCharacterId_Requesting~", x => new { x.TargetRequestCharacterId, x.RequestingCharacterId });
                    table.ForeignKey(
                        name: "FK_character_friendrequests_characters_RequestingCharacterId",
                        column: x => x.RequestingCharacterId,
                        principalTable: "characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_friendrequests_characters_TargetRequestCharacterId",
                        column: x => x.TargetRequestCharacterId,
                        principalTable: "characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_friendrequests");
        }
    }
}

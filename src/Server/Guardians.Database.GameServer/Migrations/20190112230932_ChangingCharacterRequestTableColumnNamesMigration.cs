using Microsoft.EntityFrameworkCore.Migrations;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class ChangingCharacterRequestTableColumnNamesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CharacterTwoId",
                table: "character_friendrequests",
                newName: "TargetRequestCharacterId");

            migrationBuilder.RenameColumn(
                name: "CharacterOneId",
                table: "character_friendrequests",
                newName: "RequestingCharacterId");

            migrationBuilder.RenameColumn(
                name: "FriendshipId",
                table: "character_friendrequests",
                newName: "FriendshipRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_character_friendrequests_CharacterTwoId",
                table: "character_friendrequests",
                newName: "IX_character_friendrequests_TargetRequestCharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_character_friendrequests_CharacterOneId",
                table: "character_friendrequests",
                newName: "IX_character_friendrequests_RequestingCharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetRequestCharacterId",
                table: "character_friendrequests",
                newName: "CharacterTwoId");

            migrationBuilder.RenameColumn(
                name: "RequestingCharacterId",
                table: "character_friendrequests",
                newName: "CharacterOneId");

            migrationBuilder.RenameColumn(
                name: "FriendshipRequestId",
                table: "character_friendrequests",
                newName: "FriendshipId");

            migrationBuilder.RenameIndex(
                name: "IX_character_friendrequests_TargetRequestCharacterId",
                table: "character_friendrequests",
                newName: "IX_character_friendrequests_CharacterTwoId");

            migrationBuilder.RenameIndex(
                name: "IX_character_friendrequests_RequestingCharacterId",
                table: "character_friendrequests",
                newName: "IX_character_friendrequests_CharacterOneId");
        }
    }
}

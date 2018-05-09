using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Guardians.Database.Character.Migrations
{
    public partial class AddingAlternateKeyCharacterName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CharacterName",
                table: "characters",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_characters_CharacterName",
                table: "characters",
                column: "CharacterName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_characters_CharacterName",
                table: "characters");

            migrationBuilder.AlterColumn<string>(
                name: "CharacterName",
                table: "characters",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}

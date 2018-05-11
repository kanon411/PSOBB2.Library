using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class SwitchedCreationDateGenerationToIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "characters",
                nullable: false,
                oldClrType: typeof(DateTime))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SessionCreationDate",
                table: "character_sessions",
                nullable: false,
                oldClrType: typeof(DateTime))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "characters",
                nullable: false,
                oldClrType: typeof(DateTime))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SessionCreationDate",
                table: "character_sessions",
                nullable: false,
                oldClrType: typeof(DateTime))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}

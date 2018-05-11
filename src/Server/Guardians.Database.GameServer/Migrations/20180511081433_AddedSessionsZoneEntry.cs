using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Guardians.Database.GameServer.Migrations
{
    public partial class AddedSessionsZoneEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_sessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    CharacterId = table.Column<int>(nullable: false),
                    SessionCreationDate = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    ZoneId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_sessions", x => x.SessionId);
                    table.UniqueConstraint("AK_character_sessions_AccountId", x => x.AccountId);
                    table.UniqueConstraint("AK_character_sessions_CharacterId", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "zone_endpoints",
                columns: table => new
                {
                    ZoneId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ZoneServerAddress = table.Column<string>(nullable: false),
                    ZoneServerPort = table.Column<short>(nullable: false),
                    isStatic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zone_endpoints", x => x.ZoneId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_sessions");

            migrationBuilder.DropTable(
                name: "zone_endpoints");
        }
    }
}

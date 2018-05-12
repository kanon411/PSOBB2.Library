using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Guardians.Database.GameServer.Migrations
{
	public partial class AddStoredProcedures : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(
@"CREATE PROCEDURE `create_claimed_session`(IN accountId int, IN characterId int, OUT result BIT)
BEGIN
DECLARE failed BOOL DEFAULT 0;
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET failed = 1;

# Check if accountid and characterid are valid first. Second part is more expensive.
# This is more preformant because it avoids locks/transactions until we absolutely have to
IF (0 = (SELECT COUNT(*)
		FROM `guardians.gameserver`.characters ch
		WHERE ch.CharacterId = characterId AND ch.AccountId = accountId))
THEN
	SET result = false;
ELSE
# If the accountId IS the accountId of the character then we need to start the transaction
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
	START TRANSACTION;
		IF (0 = (SELECT COUNT(*)
			FROM `guardians.gameserver`.claimed_sessions CS
			INNER JOIN characters C ON C.CharacterId = CS.CharacterId
			WHERE C.AccountId = accountId))
		THEN
			INSERT INTO `guardians.gameserver`.claimed_sessions (CharacterId) VALUES(characterId);
			SET result = true;
		ELSE
			SET result = false;
		END IF;
		
	IF failed
	THEN
		ROLLBACK;
	ELSE
		COMMIT;
	END IF;
	
END IF;
END");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			//TODO: Add drop table
		}
	}
}

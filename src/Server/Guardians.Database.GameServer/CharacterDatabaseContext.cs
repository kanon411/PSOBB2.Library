﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guardians
{
	public sealed class CharacterDatabaseContext : DbContext
	{
		public DbSet<CharacterEntryModel> Characters { get; set; }

		public DbSet<CharacterSessionModel> CharacterSessions { get; set; }

		public DbSet<ZoneInstanceEntryModel> ZoneEntries { get; set; }

		public DbSet<CharacterLocationModel> CharacterLocations { get; set; }

		public DbSet<ClaimedSessionsModel> ClaimedSession { get; set; }

		public CharacterDatabaseContext(DbContextOptions options) 
			: base(options)
		{
		}

		public CharacterDatabaseContext()
		{

		}

		//We do the below for local database creation stuff
#if DATABASE_MIGRATION
		/// <inheritdoc />
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//TODO: Should I have local or also AWS setup here?
			optionsBuilder.UseMySql("Server=localhost;Database=guardians.gameserver;Uid=root;Pwd=test;");

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			EntityTypeBuilder<CharacterEntryModel> characterEntity = modelBuilder.Entity<CharacterEntryModel>();

			//Use the below due to issue on Pomelo: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/252
			characterEntity
				.HasIndex(c => c.CharacterName)
				.IsUnique();

			characterEntity
				.HasIndex(c => c.AccountId)
				.IsUnique(false);

			//Sessions should enforce uniqueness on both character id and account id.
			EntityTypeBuilder<CharacterSessionModel> sessionEntity = modelBuilder.Entity<CharacterSessionModel>();

			//Use the below due to issue on Pomelo: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/252
			sessionEntity
				.HasIndex(s => s.CharacterId)
				.IsUnique();

			//TODO: Enforce characterid and accountid match the character
			//We no longer have a uniquness constraint here so that inactive sessions
			//from multiple characters on the account can exist
			//sessionEntity.HasAlternateKey(s => s.AccountId);

			sessionEntity
				.HasOne(s => s.CharacterEntry)
				.WithOne()
				.HasForeignKey<CharacterSessionModel>(s => s.CharacterId);

			sessionEntity
				.HasOne(s => s.ZoneEntry)
				.WithMany()
				.HasForeignKey(c => c.ZoneId);

			EntityTypeBuilder<ClaimedSessionsModel> claimedSessionModel = modelBuilder.Entity<ClaimedSessionsModel>();

			claimedSessionModel
				.HasOne(s => s.Session)
				.WithOne()
				.HasForeignKey<ClaimedSessionsModel>(s => s.CharacterId);
		}
#endif
	}
}
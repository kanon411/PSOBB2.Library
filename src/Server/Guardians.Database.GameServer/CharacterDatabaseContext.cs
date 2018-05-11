using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Guardians
{
	public sealed class CharacterDatabaseContext : DbContext
	{
		public DbSet<CharacterDatabaseModel> Characters { get; set; }

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
			optionsBuilder.UseMySql("Server=localhost;Database=guardians.characters;Uid=root;Pwd=test;");

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CharacterDatabaseModel>()
				.HasAlternateKey(c => c.CharacterName);
		}
#endif
	}
}

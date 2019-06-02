﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PSOBB
{
	public sealed class NamedEndpointDbContext : DbContext
	{
		public DbSet<NamedResolvedEndpointEntryModel> Endpoints { get; set; }

		public NamedEndpointDbContext(DbContextOptions options, [FromServices] IRegionNamedEndpointStoreRepository regionEndpointStore)
			: base(options)
		{
			if (regionEndpointStore == null) throw new ArgumentNullException(nameof(regionEndpointStore));

			//TODO: We should probably use a database, a real one, at some point.
			foreach (ClientRegionLocale region in Enum.GetValues(typeof(ClientRegionLocale)).Cast<ClientRegionLocale>())
			{
				if (regionEndpointStore.HasRegionStore(region).Result)
				{
					NameEndpointResolutionStorageModel model = regionEndpointStore.Retrieve(region).Result;
					foreach (var kvp in model.ServiceEndpoints)
						Endpoints.Add(new NamedResolvedEndpointEntryModel(model.Region, kvp.Key, kvp.Value.EndpointAddress, kvp.Value.EndpointPort));
				}
			}

			this.SaveChanges();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//Creates a composite key. We can't do this with attributes/annotations
			modelBuilder.Entity<NamedResolvedEndpointEntryModel>()
				.HasKey(c => new { c.Region, c.Service});
		}
	}
}

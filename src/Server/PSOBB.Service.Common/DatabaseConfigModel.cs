using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PSOBB
{
	[JsonObject]
	public sealed class DatabaseConfigModel
	{
		/// <summary>
		/// The connection string for the database.
		/// </summary>
		[JsonProperty]
		public string ConnectionString { get; set; }

		public DatabaseConfigModel()
		{
			
		}
	}
}

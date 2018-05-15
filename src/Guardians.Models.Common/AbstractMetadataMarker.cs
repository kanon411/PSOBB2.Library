using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Guardians
{
	public abstract class AbstractMetadataMarker<TTypeToScanAssemblyFrom>
		where TTypeToScanAssemblyFrom : class
	{
		/// <summary>
		/// The models defined in the assembly.
		/// </summary>
		public static IEnumerable<Type> ModelTypes { get; }
			= typeof(TTypeToScanAssemblyFrom)
				.GetTypeInfo()
				.Assembly
				.DefinedTypes
				.Where(t => t.GetCustomAttribute<JsonObjectAttribute>() != null)
				.Select(t => t.AsType());
	}
}

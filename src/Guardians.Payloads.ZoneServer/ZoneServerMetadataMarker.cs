using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Class that offers high-level metadata about the payload assembly.
	/// </summary>
	public static class ZoneServerMetadataMarker
	{
		/// <summary>
		/// Collection of all Zone Server payload types.
		/// </summary>
		public static IReadOnlyCollection<Type> PayloadTypes { get; }

		/// <summary>
		/// Collection of all Zone Server payload types as a dictionary
		/// with the key as the operation code.
		/// </summary>
		public static IReadOnlyDictionary<GamePayloadOperationCode, Type> PayloadTypesByOpCodeMap { get; }

		static ZoneServerMetadataMarker()
		{
			PayloadTypes = typeof(ClientSessionClaimRequestPayload)
				.Assembly
				.GetExportedTypes()
				.Where(t => t.GetCustomAttribute<GamePayloadAttribute>() != null)
				.Distinct()
				.ToArray();

			PayloadTypesByOpCodeMap = PayloadTypes
				.ToDictionary(type => type.GetCustomAttribute<GamePayloadAttribute>().OperationCode);
		}
	}
}

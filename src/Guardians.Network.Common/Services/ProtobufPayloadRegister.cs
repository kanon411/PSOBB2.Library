using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf.Meta;

namespace Guardians
{
	public sealed class ProtobufPayloadRegister
	{
		public ProtobufPayloadRegister()
		{
			
		}

		public void Register(IReadOnlyDictionary<GamePayloadOperationCode, Type> clientPayloadByOpCode, IReadOnlyDictionary<GamePayloadOperationCode, Type> serverPayloadByOpCode)
		{
			RuntimeTypeModel.Default.Add(typeof(GameClientPacketPayload), true);
			RuntimeTypeModel.Default.Add(typeof(GameServerPacketPayload), true);

			clientPayloadByOpCode
				.AsEnumerable()
				.Concat(serverPayloadByOpCode)
				.ToList()
				.ForEach(pair =>
				{
					Console.Write($"Registering Type: {pair.Value} Key: {(int)pair.Key}");

					RuntimeTypeModel.Default.Add(pair.Value, true);

					RuntimeTypeModel.Default[pair.Value.BaseType]
						.AddSubType((int)pair.Key, pair.Value);
				});
		}
	}
}

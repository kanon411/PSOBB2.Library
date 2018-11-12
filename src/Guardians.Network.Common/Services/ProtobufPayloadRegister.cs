using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf.Meta;

namespace Guardians
{
	public class ProtobufPayloadRegister
	{
		public ProtobufPayloadRegister()
		{
			
		}

		public virtual void RegisterDefaults()
		{
			//Do nothing.
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

					//TODO: Will this ever prevent a subtype registeration?
					RuntimeTypeModel.Default.Add(pair.Value, true);

					//TODO: Sometimes for unit testing this fails before the Protobuf model isn't reset. Figure a way to handle it. IfDefined breaks the Unity3D.
					RuntimeTypeModel.Default[pair.Value.BaseType]
						.AddSubType((int)pair.Key, pair.Value);
				});
		}
	}
}

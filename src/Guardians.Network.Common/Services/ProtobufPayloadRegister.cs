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
			if(!RuntimeTypeModel.Default.IsDefined(typeof(GameClientPacketPayload)))
				RuntimeTypeModel.Default.Add(typeof(GameClientPacketPayload), true);
			if(!RuntimeTypeModel.Default.IsDefined(typeof(GameServerPacketPayload)))
				RuntimeTypeModel.Default.Add(typeof(GameServerPacketPayload), true);

			clientPayloadByOpCode
				.AsEnumerable()
				.Concat(serverPayloadByOpCode)
				.ToList()
				.ForEach(pair =>
				{
					Console.Write($"Registering Type: {pair.Value} Key: {(int)pair.Key}");

					//TODO: Will this ever prevent a subtype registeration?
					if(!RuntimeTypeModel.Default.IsDefined(pair.Value))
					{
						RuntimeTypeModel.Default.Add(pair.Value, true);

						RuntimeTypeModel.Default[pair.Value.BaseType]
							.AddSubType((int)pair.Key, pair.Value);
					}
				});
		}
	}
}

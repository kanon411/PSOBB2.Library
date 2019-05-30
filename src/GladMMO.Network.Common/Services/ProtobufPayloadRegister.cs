using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ProtoBuf.Meta;

namespace GladMMO
{
	public class ProtobufPayloadRegister
	{
		private Assembly AssemblyToLoadFrom { get; }

		public ProtobufPayloadRegister([NotNull] Assembly assemblyToLoadFrom)
		{
			AssemblyToLoadFrom = assemblyToLoadFrom ?? throw new ArgumentNullException(nameof(assemblyToLoadFrom));
		}

		public virtual void RegisterDefaults()
		{
			//Do nothing.
		}

		public void Register()
		{
			RuntimeTypeModel.Default.AutoCompile = false;

			RuntimeTypeModel.Default.Add(typeof(GameClientPacketPayload), true);
			RuntimeTypeModel.Default.Add(typeof(GameServerPacketPayload), true);

			foreach(var type in AssemblyToLoadFrom.GetTypes())
			{
				GamePayloadAttribute attribute = type.GetCustomAttribute<GamePayloadAttribute>();

				if(attribute == null)
					continue;

				//TODO: Will this ever prevent a subtype registeration?
				RuntimeTypeModel.Default.Add(type, true);

				//TODO: Sometimes for unit testing this fails before the Protobuf model isn't reset. Figure a way to handle it. IfDefined breaks the Unity3D.
				RuntimeTypeModel.Default[type]
					.AddSubType((int)attribute.OperationCode, type);
			}
		}
	}
}
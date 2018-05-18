using System;
using GladNet;
using ProtoBuf;

namespace Guardians
{
	/// <summary>
	/// Base type for the game packets that clients send.
	/// </summary>
	[ProtoContract(UseProtoMembersOnly = true)]
	public abstract class GameClientPacketPayload : IPacketPayload
	{
		//We don't need anything in the base packet actually.
	}
}

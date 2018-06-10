﻿using System;
using GladNet;
using ProtoBuf;

namespace Guardians
{
	//TODO: PR to Protobuf-Net doc fix for EnumPassthru
	/// <summary>
	/// Base type for the game packets that the server send.
	/// </summary>
	[ProtoContract(UseProtoMembersOnly = true)]
	public abstract class GameServerPacketPayload : IPacketPayload
	{
		//We don't need anything in the base packet actually.
	}
}
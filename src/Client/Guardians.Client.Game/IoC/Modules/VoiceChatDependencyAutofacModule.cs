﻿using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Logging;
using Dissonance.GladNet;
using Dissonance.Networking;
using GladNet;
using ProtoBuf;

namespace Guardians
{
	public sealed class VoiceChatDependencyAutofacModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<GladNetDissonanceClient>()
				.AsSelf();

			builder.RegisterType<GladNetDissonanceClientCurrentInstanceAdapter>()
				.As<IVoiceGateway>()
				.As<IVoiceDataProcessor>()
				.AsSelf()
				.SingleInstance();
		}
	}
}
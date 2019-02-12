using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Guardians
{
	public interface IChatChannelAssociatable
	{
		[JsonProperty]
		ChatChannels TargetChannel { get; }
	}
}

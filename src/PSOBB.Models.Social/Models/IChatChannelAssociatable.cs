using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PSOBB
{
	public interface IChatChannelAssociatable
	{
		[JsonProperty]
		ChatChannels TargetChannel { get; }
	}
}

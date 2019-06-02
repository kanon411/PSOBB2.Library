using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GladMMO
{
	public interface IChatChannelAssociatable
	{
		[JsonProperty]
		ChatChannels TargetChannel { get; }
	}
}

using System;
using System.ComponentModel;
using Dissonance.Audio.Codecs;
using ProtoBuf;

namespace Dissonance
{
	public static class CodecDefaults
	{
		//Code: Opus FrameSize: 1920 SampleRate: 48000
		[ProtoIgnore]
		public static readonly CodecSettings Default = new CodecSettings(Codec.Opus, 1920, 48000);
	}

	//TODO: Doc from Dissonance
	/// <summary>
	/// The Codec settings structure from
	/// Dissonance.
	/// </summary>
	[ProtoContract]
	public struct CodecSettings
	{
		[ProtoMember(1)]
		private readonly Codec _codec;

		[ProtoMember(2)]
		private readonly uint _frameSize;

		[ProtoMember(3)]
		private readonly int _sampleRate;

		[ProtoIgnore]
		public Codec Codec => _codec;

		[ProtoIgnore]
		public uint FrameSize => _frameSize;

		[ProtoIgnore]
		public int SampleRate => _sampleRate;

		public CodecSettings(Codec codec, uint frameSize, int sampleRate)
		{
			//TODO: Will this cause perf issues?
			if(!Enum.IsDefined(typeof(Codec), codec)) throw new InvalidEnumArgumentException(nameof(codec), (int)codec, typeof(Codec));

			_codec = codec;
			_frameSize = frameSize;
			_sampleRate = sampleRate;
		}

		public override string ToString()
		{
			return string.Format("Codec: {0}, FrameSize: {1}, SampleRate: {2:##.##}kHz", Codec, FrameSize, SampleRate / 1000f);
		}
	}
}

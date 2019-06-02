﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PSOBB
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class GamePayloadAttribute : Attribute
	{
		/// <summary>
		/// The operation code.
		/// </summary>
		public GamePayloadOperationCode OperationCode { get; }

		/// <inheritdoc />
		public GamePayloadAttribute(GamePayloadOperationCode operationCode)
		{
			if(!Enum.IsDefined(typeof(GamePayloadOperationCode), operationCode)) throw new InvalidEnumArgumentException(nameof(operationCode), (int)operationCode, typeof(GamePayloadOperationCode));
			OperationCode = operationCode;
		}

		protected GamePayloadAttribute()
		{
			
		}
	}
}

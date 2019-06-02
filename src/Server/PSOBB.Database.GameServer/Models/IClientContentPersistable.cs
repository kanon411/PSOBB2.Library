﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PSOBB
{
	public interface IClientContentPersistable
	{
		//TODO: Should we like tables somehow with auth? Probably not?
		/// <summary>
		/// Key for the account associated with the creation/registeration of this world.
		/// </summary>
		[Required]
		[Range(0, int.MaxValue)]
		int AccountId { get; }

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		DateTime CreationTime { get; }

		[Required]
		[StringLength(15, MinimumLength = 7)] //IP constraints
		string CreationIp { get; }

		//TODO: Add public/private/uploading/uploaded state field
		/// <summary>
		/// The GUID name of the stored content.
		/// </summary>
		[Required]
		Guid StorageGuid { get; }
	}
}

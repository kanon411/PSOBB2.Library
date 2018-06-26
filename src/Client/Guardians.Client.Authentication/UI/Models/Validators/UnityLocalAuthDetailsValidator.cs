using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Guardians;

namespace Guardians
{
	public sealed class UnityLocalAuthDetailsValidator : AbstractValidator<IUserAuthenticationDetailsContainer>
	{
		public UnityLocalAuthDetailsValidator()
		{
			RuleFor(c => c.Password).MinimumLength(3).MaximumLength(64).NotNull();
			RuleFor(c => c.UserName).MinimumLength(3).MaximumLength(64).NotNull();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB
{
	public interface ILoginButtonClickedEventSubscribable
	{
		event EventHandler OnLoginButtonClicked;
	}
}

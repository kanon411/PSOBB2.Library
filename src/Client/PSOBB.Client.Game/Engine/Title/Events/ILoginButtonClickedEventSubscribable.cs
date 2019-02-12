using System;
using System.Collections.Generic;
using System.Text;

namespace PSOBB.Client
{
	public interface ILoginButtonClickedEventSubscribable
	{
		event EventHandler OnLoginButtonClicked;
	}
}

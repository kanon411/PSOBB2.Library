using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
//using PostSharp.Patterns.Caching;
//using PostSharp.Patterns.Caching.Backends;
using SceneJect.Common;
using TypeSafe.Http.Net;
using UnityEngine;

namespace Guardians
{
	//TODO: Extract this into an Autofac module
	public sealed class GuardiansUnityCharacterScreenDependencyContainer : AutofacBasedDependencyRegister<CharacterScreenDependencyAutofacModule>
	{
		[SerializeField]
		private string ServiceDiscoveryUrl;

		/// <inheritdoc />
		protected override CharacterScreenDependencyAutofacModule CreateModule()
		{
			//We need to provide the service discovery url
			return new CharacterScreenDependencyAutofacModule(ServiceDiscoveryUrl);
		}
	}
}

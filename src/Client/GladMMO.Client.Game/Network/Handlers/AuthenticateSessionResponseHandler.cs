using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using FreecraftCore;
using GladMMO;
using GladNet;

namespace GladMMO
{
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class AuthenticateSessionResponseHandler : BaseGameClientGameMessageHandler<AuthenticateSessionResponse>
	{
		/// <inheritdoc />
		public AuthenticateSessionResponseHandler(ILog logger) 
			: base(logger)
		{

		}

		/// <inheritdoc />
		public override async Task HandleMessage(IPeerMessageContext<GamePacketPayload> context, AuthenticateSessionResponse payload)
		{
			Logger.Info($"Recieved Auth Response Result: {payload.AuthenticationResult}");

			//HelloKitty: Once we've got an auth response, if it's success we actually don't even need the character list. We should just try to login to the first character.
			//This will allow for preventing any performance impact querying for character list on login.
			if(payload.AuthenticationResult == SessionAuthenticationResult.AUTH_OK)
			{
				//We can't just log into a character sadly, TC only lets us log into seen/known characters.
				await context.PayloadSendService.SendMessage(new CharacterListRequest());
			}
			else
				await context.ConnectionService.DisconnectAsync(0); //TODO: Log.
		}
	}
}
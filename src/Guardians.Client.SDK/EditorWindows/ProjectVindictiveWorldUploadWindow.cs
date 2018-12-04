using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Guardians.SDK
{
	public sealed class ProjectVindictiveWorldUploadWindow : EditorWindow
	{
		[SerializeField]
		private string AccountName;

		[SerializeField]
		private string Password;

		[SerializeField]
		private string AuthToken;

		[SerializeField]
		private UnityEngine.Object SceneObject;

		[SerializeField]
		private string AssetBundlePath;

		[MenuItem("ProjectVindictive/WorldUpload")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(ProjectVindictiveWorldUploadWindow));
		}

		void OnGUI()
		{
			AccountName = EditorGUILayout.TextField("Account", AccountName);
			Password = EditorGUILayout.PasswordField("Password", Password);

			//TODO: Validate scene file
			SceneObject = EditorGUILayout.ObjectField("Scene", SceneObject, typeof(SceneAsset), false);

			if(GUILayout.Button("Build World AssetBundle"))
			{
				if(!Authenticate())
				{
					Debug.LogError($"Failed to authenticate User: {AccountName}");
					return;
				}

				//Once authenticated we need to try to build the bundle.
				ProjectVindictiveAssetbundleBuilder builder = new ProjectVindictiveAssetbundleBuilder(SceneObject);

				//TODO: Handle uploading build
				AssetBundleManifest manifest = builder.BuildBundle();

				//TODO: Refactor all this crap
				AssetBundlePath = manifest.GetAllAssetBundles().First();

				Debug.Log($"Generated AssetBundle with Path: {AssetBundlePath}");

				return;
			}

			if(GUILayout.Button("Upload Assetbundle"))
			{
				//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
				ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

				IContentServerServiceClient ucmService = Refit.RestService.For<IContentServerServiceClient>("http://localhost:5005/");

				//Done out here, must be called on the main thread
				string projectPath = Application.dataPath.ToLower().TrimEnd(@"/assets".ToCharArray());

				Thread uploadThread = new Thread(new ThreadStart(async () =>
				{
					try
					{
						Debug.Log("Requesting URL.");
						//HttpWebRequest httpRequest = WebRequest.Create(ucmService.GetNewWorldUploadUrl(AuthToken).Result.UploadUrl) as HttpWebRequest;
						HttpWebRequest httpRequest = WebRequest.Create((await ucmService.GetNewWorldUploadUrl(AuthToken)).UploadUrl) as HttpWebRequest;
						Debug.Log("Built http request with URL");

						httpRequest.Method = "PUT";
						using(Stream dataStream = httpRequest.GetRequestStream())
						{
							
							byte[] buffer = new byte[8000];
							using(FileStream fileStream = new FileStream(Path.Combine(projectPath, "AssetBundles", "temp", AssetBundlePath), FileMode.Open, FileAccess.Read))
							{
								int bytesRead = 0;
								while((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
								{
									dataStream.Write(buffer, 0, bytesRead);
								}
							}
						}

						HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
					}
					catch(Exception e)
					{
						Debug.LogError($"{e.Message}\n\n{e.StackTrace}");
						throw;
					}
				}));

				uploadThread.Start();
			}
		}

		private bool Authenticate()
		{
			//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

			//TODO: Service discovery
			IAuthenticationService authService = Refit.RestService.For<IAuthenticationService>("http://localhost:5001/");

			//Authentication using provided credentials
			JWTModel result = authService.TryAuthenticate(new AuthenticationRequestModel(AccountName, Password)).Result;

			Debug.Log($"Auth Result: {result.isTokenValid} Token: {result.AccessToken} Error: {result.Error} ErrorDescription: {result.ErrorDescription}.");

			AuthToken = $"Bearer {result.AccessToken}";

			return result.isTokenValid;
		}

		//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
		private bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			return true;
		}
	}
}
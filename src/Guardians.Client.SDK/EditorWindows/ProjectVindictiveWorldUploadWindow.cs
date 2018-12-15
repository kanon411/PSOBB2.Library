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
	public sealed class ProjectVindictiveWorldUploadWindow : AuthenticatableEditorWindow
	{
		[SerializeField]
		private UnityEngine.Object SceneObject;

		[SerializeField]
		private string AssetBundlePath;

		[MenuItem("ProjectVindictive/WorldUpload")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(ProjectVindictiveWorldUploadWindow));
		}

		protected override void OnGUI()
		{
			base.OnGUI();

			//TODO: Validate scene file
			SceneObject = EditorGUILayout.ObjectField("Scene", SceneObject, typeof(SceneAsset), false);

			if(GUILayout.Button("Build World AssetBundle"))
			{
				if(!TryAuthenticate())
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
				string projectPath = Application.dataPath.ToLower().TrimEnd(@"assets".ToCharArray());

				Thread uploadThread = new Thread(new ThreadStart(async () =>
				{
					try
					{
						Debug.Log("Requesting URL.");
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

					Debug.Log($"Finished uploading content.");
				}));

				uploadThread.Start();
			}
		}
	}
}
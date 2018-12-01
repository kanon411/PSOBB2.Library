using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Guardians.SDK
{
	public sealed class ProjectVindictiveAssetbundleBuilder
	{
		/// <summary>
		/// The asset to build the bundle for.
		/// </summary>
		private UnityEngine.Object ObjectToBundle { get; }

		/// <summary>
		/// Creates a new builder object that can generate
		/// asset bundles for the provided object.
		/// </summary>
		/// <param name="objectToBundle"></param>
		public ProjectVindictiveAssetbundleBuilder(Object objectToBundle)
		{
			if(objectToBundle == null) throw new ArgumentNullException(nameof(objectToBundle), "Cannot build a NULL asset to an asset bundle.");

			ObjectToBundle = objectToBundle;
		}

		public AssetBundleManifest BuildBundle()
		{
			//We want to build the current scene as an asset bundle
			AssetBundleBuild bundle = new AssetBundleBuild();

			string assetPath = AssetDatabase.GetAssetPath(ObjectToBundle);

			if(string.IsNullOrEmpty(assetPath))
				throw new InvalidOperationException($"Failed to get asset path for {nameof(UnityEngine.Object)}: {ObjectToBundle.name}. Cannot build asset bundle.");

			//TODO: Implement better name handling
			bundle.assetBundleName = Guid.NewGuid().ToString();
			bundle.assetBundleVariant = "default";
			bundle.assetNames = new string[1] { assetPath };

			//TODO: Is it ok to specify build target like this?
			return BuildPipeline.BuildAssetBundles("AssetBundles/temp/", new AssetBundleBuild[1] { bundle }, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
		}
	}
}
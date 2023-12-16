using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using DigitalRuby.ThunderAndLightning;
using LC_API.Extensions;
using Steamworks.Data;
using UnityEngine;

//todo: force all paths to lowercase after privating 'assets' to ensure all paths get normalized when accessing.

#pragma warning disable CS0618 // Type or member is obsolete - we're the ones obsoleting things.
namespace LC_API.BundleAPI
{
    /// <summary>
    /// Use the BundleLoader to load and get custom assets. A folder for placing custom assets will be generated in BepInEx/Bundles.
    /// </summary>
    public static class BundleLoader
    {
        /// <summary>
        /// Is set to <see langword="true"/> if there are any files not ending in '.manifest' in the legacy bundle dir. <see langword="false"/> if there are no files or only .manifest files.
        /// </summary>
        public static bool AssetsInLegacyDirectory { get; private set; }

        /// <summary>
        /// Is set to <see langword="true"/> if asset loading legacy bundle dir was enabled. <see langword="false"/> if assets were not allowed to be loaded from the legacy bundle dir.
        /// </summary>
        public static bool LegacyLoadingEnabled { get; private set; }

        // i think eventually 'assets' should be privatized and changed to a Dictionary. Unity is comically thread-unsafe, so using a thread-safe ConcurrentDict is overkill.
        [Obsolete("Use GetLoadedAsset instead. This will be removed/private in a future update.")]
        public static ConcurrentDictionary<string, UnityEngine.Object> assets = new ConcurrentDictionary<string, UnityEngine.Object>();

        [Obsolete("Use OnLoadedBundles instead. This will be removed/private in a future update.")]
        public delegate void OnLoadedAssetsDelegate();
        /// <summary>
        /// This is called when the BundleLoader finishes loading assets.
        /// </summary>
        [Obsolete("Use OnLoadedBundles instead. This will be removed/private in a future update.")]
        public static OnLoadedAssetsDelegate OnLoadedAssets = LoadAssetsCompleted;

        /// <summary>
        /// This is called when the BundleLoader finishes loading assets.
        /// </summary>
        public static event Action OnLoadedBundles = LoadAssetsCompleted;

        private static Dictionary<string, Dictionary<string, UnityEngine.Object>> loadedAssetBundles = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

        internal static void Load(bool legacyLoading)
        {
            LegacyLoadingEnabled = legacyLoading;
            if (LegacyLoadingEnabled)
            {
                Plugin.Log.LogMessage("BundleAPI will now load all asset bundles...");
                string bundleDir = Path.Combine(Paths.BepInExRootPath, "Bundles");
                if (!Directory.Exists(bundleDir))
                {
                    Directory.CreateDirectory(bundleDir);
                    Plugin.Log.LogMessage("BundleAPI Created legacy bundle directory in BepInEx/Bundles");
                }
                string[] bundles = Directory.GetFiles(bundleDir, "*", SearchOption.AllDirectories).Where(x => !x.EndsWith(".manifest", StringComparison.CurrentCultureIgnoreCase)).ToArray();

                AssetsInLegacyDirectory = bundles.Length != 0;

                if (!AssetsInLegacyDirectory)
                {
                    Plugin.Log.LogMessage("BundleAPI got no assets to load from legacy directory");
                }

                if (AssetsInLegacyDirectory)
                {
                    Plugin.Log.LogWarning("The path BepInEx > Bundles is outdated and should not be used anymore! Bundles will be loaded from BepInEx > plugins from now on");
                    LoadAllAssetsFromDirectory(bundles, legacyLoading);
                }

                string[] invalidEndings = { ".dll", ".json", ".png", ".md", ".old", ".txt", ".exe", ".lem" };
                bundleDir = Path.Combine(Paths.BepInExRootPath, "plugins");
                bundles = Directory.GetFiles(bundleDir, "*", SearchOption.AllDirectories).Where(file => !invalidEndings.Any(ending => file.EndsWith(ending, StringComparison.CurrentCultureIgnoreCase))).ToArray();

                byte[] bundleStart = Encoding.ASCII.GetBytes("UnityFS");

                List<string> properBundles = new List<string>();

                foreach (string path in bundles)
                {
                    byte[] buffer = new byte[bundleStart.Length];

                    using (FileStream fs = File.Open(path, FileMode.Open))
                    {
                        fs.Read(buffer, 0, buffer.Length);
                    }

                    if (buffer.SequenceEqual(bundleStart))
                    {
                        properBundles.Add(path);
                    }
                }

                bundles = properBundles.ToArray();

                if (bundles.Length == 0)
                {
                    Plugin.Log.LogMessage("BundleAPI got no assets to load from plugins folder");
                }
                else
                {
                    LoadAllAssetsFromDirectory(bundles, legacyLoading);
                }

                OnLoadedAssets.InvokeParameterlessDelegate();
                OnLoadedBundles.InvokeActionSafe();
            }
        }

        private static void LoadAllAssetsFromDirectory(string[] array, bool legacyLoading)
        {
            if (legacyLoading)
            {
                Plugin.Log.LogMessage("BundleAPI got " + array.Length.ToString() + " AssetBundles to load!");

                for (int i = 0; i < array.Length; i++)
                {
                    try
                    {
                        SaveAsset(array[i], legacyLoading);
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log.LogError("Failed to load an assetbundle! Path: " + array[i]);
                    }
                }
            }
            else
            {
                Plugin.Log.LogMessage("BundleAPI got " + array.Length.ToString() + " AssetBundles to load!");

                for (int i = 0; i < array.Length; i++)
                {
                    try
                    {
                        SaveAsset(array[i], legacyLoading);
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log.LogError("Failed to load an assetbundle! Path: " + array[i]);
                    }
                }
            }
            
        }

        /// <summary>
        /// Saves all assets from a bundle at the given path. It is not recommended to call this method, as it can introduce instability.
        /// </summary>
        public static void SaveAsset(string path, bool legacyLoad)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            try
            {
                string[] assetPaths = bundle.GetAllAssetNames();
                //Plugin.Log.LogMessage("Assets: [" + string.Join(", ", array) + "]");
                foreach (string assetPath in assetPaths)
                {
                    Plugin.Log.LogMessage("Got asset for load: " + assetPath);
                    UnityEngine.Object loadedAsset = bundle.LoadAsset(assetPath);
                    if (loadedAsset == null)
                    {
                        Plugin.Log.LogWarning($"Skipped/failed loading an asset (from bundle '{bundle.name}') - Asset path: {loadedAsset}");
                        continue;
                    }

                    string text2 = legacyLoad ? assetPath.ToUpper() : assetPath.ToLower();

                    if (assets.ContainsKey(text2))
                    {
                        Plugin.Log.LogError("BundleAPI got duplicate asset!");
                        return;
                    }
                    assets.TryAdd(text2, loadedAsset);
                    Plugin.Log.LogMessage("Loaded asset: " + loadedAsset.name);
                }
            }
            finally
            {
                bundle?.Unload(false);
            }
        }

        /// <summary>
        /// Get an asset from all the loaded assets. Use this to access your custom assets. Returned value may be null.
        /// </summary>
        public static TAsset GetLoadedAsset<TAsset>(string itemPath) where TAsset : UnityEngine.Object
        {
            UnityEngine.Object? asset = null;
            if (LegacyLoadingEnabled)
                assets.TryGetValue(itemPath.ToUpper(), out asset);

            if (asset == null)
                assets.TryGetValue(itemPath.ToLower(), out asset);

            return (TAsset)asset;
        }

        /// <summary>
        /// Loads an entire asset bundle. It is recommended to use this to load asset bundles.
        /// </summary>
        /// <param name="filePath">The file system path of the asset bundle.</param>
        /// <param name="cache">Whether or not to cache the loaded bundle.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing all assets from the bundle mapped to their path in lowercase.</returns>
        public static Dictionary<string, UnityEngine.Object> LoadAssetBundle(string filePath, bool cache = true)
        {
            if (loadedAssetBundles.TryGetValue(filePath, out Dictionary<string, UnityEngine.Object> _assets)) 
                return _assets;

            Dictionary<string, UnityEngine.Object> assetPairs = new Dictionary<string, UnityEngine.Object>();

            // Create a shallow clone so that anyone who edits the returned dictionary doesn't mess up the cached version.
            Dictionary<string, UnityEngine.Object> toSave = null;
            
            if (cache) toSave = new Dictionary<string, UnityEngine.Object>();

            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            try
            {
                string[] assetPaths = bundle.GetAllAssetNames();
                //Plugin.Log.LogMessage("Assets: [" + string.Join(", ", array) + "]");
                foreach (string assetPath in assetPaths)
                {
                    UnityEngine.Object loadedAsset = bundle.LoadAsset(assetPath);
                    if (loadedAsset == null)
                    {
                        Plugin.Log.LogWarning($"Skipped/failed loading an asset (from bundle '{bundle.name}') - Asset path: {loadedAsset}");
                        continue;
                    }

                    string path = assetPath.ToLower();

                    if (assets.ContainsKey(path))
                    {
                        Plugin.Log.LogError("BundleAPI got duplicate asset!");
                        return null;
                    }

                    assets.TryAdd(path, loadedAsset);

                    assetPairs.Add(path, loadedAsset);
                    if (cache) toSave.Add(path, loadedAsset);
                }
            }
            finally
            {
                bundle?.Unload(false);
            }

            if (cache) loadedAssetBundles.Add(filePath, toSave);

            return assetPairs;
        }

        private static void LoadAssetsCompleted()
        {
            Plugin.Log.LogMessage("BundleAPI finished loading all assets.");
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete - we're the ones obsoleting things.

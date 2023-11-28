using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using DigitalRuby.ThunderAndLightning;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.Android;

namespace LC_API.BundleAPI
{
    /// <summary>
    /// Use the BundleLoader to load and get custom assets. A folder for placing custom assets will be generated in BepInEx/Bundles.
    /// </summary>
    public class BundleLoader
    {
        public static ConcurrentDictionary<string, UnityEngine.Object> assets;
        public static bool assetsInLegacyDirectory;
        public static bool legacyLoadingEnabled;
        public delegate void OnLoadedAssetsDelegate();

        /// <summary>
        /// This is called when the BundleLoader finishes loading assets.
        /// </summary>
        public static OnLoadedAssetsDelegate OnLoadedAssets;

        /// <summary>
        /// Loads all asset bundles present in the bundle directory. It is not recommended to call this method, as it will introduce instability.
        /// </summary>
        public static void Load(bool legacyLoading)
        {
            assetsInLegacyDirectory = true;
            legacyLoadingEnabled = legacyLoading;
            assets = new ConcurrentDictionary<string, UnityEngine.Object>();
            Plugin.Log.LogMessage("BundleAPI will now load all asset bundles...");
            string dir1 = Path.Combine(Paths.BepInExRootPath, "Bundles");
            if (!Directory.Exists(dir1))
            {
                Directory.CreateDirectory(dir1);
                Plugin.Log.LogMessage("BundleAPI Created legacy bundle directory in BepInEx/Bundles");
            }
            string[] array = Directory.GetFiles(dir1, "*", SearchOption.AllDirectories).Where(x => !x.EndsWith(".manifest", StringComparison.CurrentCultureIgnoreCase)).ToArray();

            if (array.Length == 0)
            {
                Plugin.Log.LogMessage("BundleAPI got no assets to load from legacy directory");
                assetsInLegacyDirectory = false;
            }
            if (assetsInLegacyDirectory)
            {
                Plugin.Log.LogWarning("The path BepInEx > Bundles is outdated and should not be used anymore! Bundles will be loaded from BepInEx > plugins from now on");
                LoadAllAssetsFromDirectory(array, legacyLoading);
            }
            dir1 = Path.Combine(Paths.BepInExRootPath, "plugins");
            array = Directory.GetFiles(dir1, "*", SearchOption.AllDirectories).Where(x => !x.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)).Where(x => !x.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase)).Where(x => !x.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase)).Where(x => !x.EndsWith(".md", StringComparison.CurrentCultureIgnoreCase)).Where(x => !x.EndsWith(".old", StringComparison.CurrentCultureIgnoreCase)).Where(x => !x.EndsWith(".txt", StringComparison.CurrentCultureIgnoreCase)).ToArray();

            if (array.Length == 0)
            {
                Plugin.Log.LogMessage("BundleAPI got no assets to load from plugins folder");
            }
            else
            {
                LoadAllAssetsFromDirectory(array, legacyLoading);
            }
            OnLoadedAssets += LoadAssetsCompleted;
            OnLoadedAssets();
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
            string[] array = bundle.GetAllAssetNames();
            //Plugin.Log.LogMessage("Assets: [" + string.Join(", ", array) + "]");
            foreach (string text in array)
            {
                Plugin.Log.LogMessage("Got asset for load: " + text);
                UnityEngine.Object obj = bundle.LoadAsset(text);
                if (obj != null)
                {
                    
                    string text2 = text.ToLower();
                    if (legacyLoad)
                    {
                        text2 = text2.ToUpper();
                    }
                    if (assets.ContainsKey(text2))
                    {
                        Plugin.Log.LogError("BundleAPI got duplicate asset!");
                        return;
                    }
                    assets.TryAdd(text2, obj);
                    Plugin.Log.LogMessage("Loaded asset: " + obj.name);
                }
                else
                {
                    Plugin.Log.LogWarning("Skipped loading an asset");
                }
            }
        }

        /// <summary>
        /// Get an asset from all the loaded assets. Use this to access your custom assets.
        /// </summary>
        public static AssetType GetLoadedAsset<AssetType>(string itemPath) where AssetType : UnityEngine.Object
        {
            UnityEngine.Object @object;
            if (legacyLoadingEnabled)
            {
                assets.TryGetValue(itemPath.ToUpper(), out @object);
            }
            else
            {
                assets.TryGetValue(itemPath.ToLower(), out @object);
            }
            UnityEngine.Object asset = @object;
            return (AssetType)asset;
        }

        private static void LoadAssetsCompleted()
        {
            Plugin.Log.LogMessage("BundleAPI finished loading all assets.");
        }

        void Awake()
        {
        }
    }
}

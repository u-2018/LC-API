using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.Android;

namespace LC_API.BundleAPI
{
    public class BundleLoader
    {
        public static ConcurrentDictionary<string, UnityEngine.Object> assets;
        public delegate void OnLoadedAssetsDelegate();

        public static OnLoadedAssetsDelegate OnLoadedAssets;

        public static void Load()
        {
            assets = new ConcurrentDictionary<string, UnityEngine.Object>();
            Plugin.Log.LogMessage("BundleAPI will now load all asset bundles...");
            string dir1 = Path.Combine(Paths.BepInExRootPath, "Bundles");
            if (!Directory.Exists(dir1))
            {
                Directory.CreateDirectory(dir1);
                Plugin.Log.LogMessage("BundleAPI Created bundle directory in BepInEx/Bundles");
            }
            string[] array = Directory.GetFiles(dir1, "*", SearchOption.AllDirectories)
                         .Where(x => !x.EndsWith(".manifest", StringComparison.CurrentCultureIgnoreCase))
                         .ToArray();

            if (array.Length == 0)
            {
                Plugin.Log.LogMessage("BundleAPI got no assets to load, stopping loading!");
                return;
            }
            Plugin.Log.LogMessage("BundleAPI got " + array.Length.ToString() + " AssetBundles to load!");
            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    SaveAsset(array[i]);
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError("Failed to load an assetbundle! Path: " + array[i]);
                }
            }
            OnLoadedAssets += LoadAssetsCompleted;
            OnLoadedAssets();
        }

        public static void SaveAsset(string path)
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
                    string text2 = text.ToUpper();
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

        public static AssetType GetLoadedAsset<AssetType>(string itemPath) where AssetType : UnityEngine.Object
        {
            UnityEngine.Object @object;
            assets.TryGetValue(itemPath.ToUpper(), out @object);
            UnityEngine.Object asset = @object;
            return (AssetType)asset;
        }

        public static void LoadAssetsCompleted()
        {
            Plugin.Log.LogMessage("BundleAPI finished loading all assets.");
        }

        void Awake()
        {
        }
    }
}

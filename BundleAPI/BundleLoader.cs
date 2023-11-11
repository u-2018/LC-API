// -----------------------------------------------------------------------
// <copyright file="BundleLoader.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LC_API.BundleAPI;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

using BepInEx;
using UnityEngine;

// Unused items.
#pragma warning disable SA1401

// "assets" should be named "Assets"
#pragma warning disable SA1307

/// <summary>
/// Use the BundleLoader to load and get custom assets. A folder for placing custom assets will be generated in BepInEx/Bundles.
/// </summary>
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
public sealed class BundleLoader
{
    /// <summary>
    /// Contains a thread-safe dictionary with custom assets.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static ConcurrentDictionary<string, UnityEngine.Object> assets;

    /// <summary>
    /// This is called when the BundleLoader finishes loading assets.
    /// </summary>
    public static OnLoadedAssetsDelegate OnLoadedAssets;

    /// <summary>
    /// The delegate for <see cref="BundleLoader.OnLoadedAssets"/>.
    /// </summary>
    public delegate void OnLoadedAssetsDelegate();

    /// <summary>
    /// Loads all asset bundles present in the bundle directory. It is not recommended to call this method, as it will introduce instability.
    /// </summary>
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
        foreach (string t in array)
        {
            try
            {
                SaveAsset(t);
            }
            catch (Exception)
            {
                Plugin.Log.LogError("Failed to load an asset-bundle! Path: " + t);
            }
        }

        OnLoadedAssets += LoadAssetsCompleted;
        OnLoadedAssets();
    }

    /// <summary>
    /// Saves all assets from a bundle at the given path. It is not recommended to call this method, as it can introduce instability.
    /// </summary>
    /// <param name="path">The path to save the asset to.</param>
    public static void SaveAsset(string path)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        string[] array = bundle.GetAllAssetNames();

        // Plugin.Log.LogMessage("Assets: [" + string.Join(", ", array) + "]");
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

    /// <summary>
    /// Get an asset from all the loaded assets. Use this to access your custom assets.
    /// </summary>
    /// <param name="itemPath">The path of the item.</param>
    /// <typeparam name="TAssetType">The type of the asset to find.</typeparam>
    /// <returns>The asset found.</returns>
    public static TAssetType GetLoadedAsset<TAssetType>(string itemPath)
        where TAssetType : UnityEngine.Object
    {
        assets.TryGetValue(itemPath.ToUpper(), out UObject @object);
        UnityEngine.Object asset = @object;
        return (TAssetType)asset;
    }

    /// <summary>
    /// Called when all assets have been unloaded.
    /// </summary>
    private static void LoadAssetsCompleted()
    {
        Plugin.Log.LogMessage("BundleAPI finished loading all assets.");
    }
}
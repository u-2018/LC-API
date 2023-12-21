using System.Collections.Generic;

namespace LC_API.BundleAPI
{
    /// <summary>
    /// A loaded asset bundle.
    /// </summary>
    public class LoadedAssetBundle
    {
        private Dictionary<string, UnityEngine.Object> _loadedAssets;

        internal LoadedAssetBundle(Dictionary<string, UnityEngine.Object> loadedAssets)
        {
            _loadedAssets = loadedAssets;
        }

        /// <summary>
        /// Gets an asset from the bundle.
        /// </summary>
        /// <typeparam name="TAsset">The type of the asset. Usually <see cref="UnityEngine.GameObject"/>.</typeparam>
        /// <param name="path">The path to the asset in the bundle.</param>
        /// <returns>The asset.</returns>
        public TAsset GetAsset<TAsset>(string path) where TAsset : UnityEngine.Object
        {
            string lowerPath = path.ToLower();

            if (_loadedAssets.TryGetValue(lowerPath, out UnityEngine.Object obj))
            {
                return obj as TAsset;
            }

            return null;
        }

        /// <summary>
        /// Tries to get an asset from the bundle.
        /// </summary>
        /// <typeparam name="TAsset">The type of the asset. Usually <see cref="UnityEngine.GameObject"/>.</typeparam>
        /// <param name="path">The path to the asset in the bundle.</param>
        /// <param name="asset">Outputs the asset.</param>
        /// <returns><see langword="true"/> if the asset is found. <see langword="false"/> if the asset isn't found, or couldn't be casted to TAsset</returns>
        public bool TryGetAsset<TAsset>(string path, out TAsset asset) where TAsset : UnityEngine.Object
        {
            string lowerPath = path.ToLower();

            asset = null;

            if (_loadedAssets.TryGetValue(lowerPath, out UnityEngine.Object obj))
            {
                if (obj is TAsset tasset) asset = tasset;
            }

            return asset != null;
        }
    }
}

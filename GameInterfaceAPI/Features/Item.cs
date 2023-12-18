using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

namespace LC_API.GameInterfaceAPI.Features
{
    public class Item : NetworkBehaviour
    {
        internal static GameObject ItemNetworkPrefab { get; set; }

        /// <summary>
        /// Gets a dictionary containing all <see cref="Item"/>s that are currently spawned in the world.
        /// </summary>
        public static Dictionary<GrabbableObject, Item> Dictionary { get; } = new Dictionary<GrabbableObject, Item>();

        /// <summary>
        /// Gets a list containing all <see cref="Item"/>s.
        /// </summary>
        public static IReadOnlyCollection<Item> List => Dictionary.Values;

        public GrabbableObject GrabbableObject { get; private set; }

        private void Awake()
        {
            GrabbableObject = GetComponent<GrabbableObject>();

            Dictionary.Add(GrabbableObject, this);
        }

        /// <summary>
        /// For internal use. Do not use.
        /// </summary>
        public override void OnDestroy()
        {
            Dictionary.Remove(GrabbableObject);

            base.OnDestroy();
        }
    }
}

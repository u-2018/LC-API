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
    /// <summary>
    /// Encapsulates a <see cref="global::GrabbableObject"/> for easier interacting.
    /// </summary>
    public class Item : NetworkBehaviour
    {
        internal static GameObject ItemNetworkPrefab { get; set; }

        /// <summary>
        /// Gets a dictionary containing all <see cref="Item"/>s that are currently spawned in the world or in <see cref="Player"/>s' inventories.
        /// </summary>
        public static Dictionary<GrabbableObject, Item> Dictionary { get; } = new Dictionary<GrabbableObject, Item>();

        /// <summary>
        /// Gets a list containing all <see cref="Item"/>s.
        /// </summary>
        public static IReadOnlyCollection<Item> List => Dictionary.Values;

        /// <summary>
        /// Gets the encapsulated <see cref="global::GrabbableObject"/>
        /// </summary>
        public GrabbableObject GrabbableObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="Item"/>'s see item properties.
        /// </summary>
        public global::Item ItemProperties => GrabbableObject?.itemProperties;

        private ScanNodeProperties ScanNodeProperties { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Item"/>'s name.
        /// </summary>
        /// <exception cref="Exception">Thrown when attempting to set item name from the client.</exception>
        public string Name
        {
            get
            {
                return GrabbableObject.itemProperties.itemName;
            }
            set
            {
                if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
                {
                    throw new Exception("Tried to set item name on client.");
                }

                string current = GrabbableObject.itemProperties.itemName.ToLower();

                GrabbableObject.itemProperties.itemName = value;
                OverrideTooltips(current, value.ToLower());

                ScanNodeProperties.headerText = value;
                SetGrabbableNameClientRpc(value);
            }
        }

        [ClientRpc]
        private void SetGrabbableNameClientRpc(string name)
        {
            string current = GrabbableObject.itemProperties.itemName.ToLower();
            GrabbableObject.itemProperties.itemName = name;
            OverrideTooltips(current, name.ToLower());

            ScanNodeProperties.headerText = name;
        }

        private void OverrideTooltips(string oldName, string newName)
        {
            for (int i = 0; i < GrabbableObject.itemProperties.toolTips.Length; i++)
            {
                GrabbableObject.itemProperties.toolTips[i] = GrabbableObject.itemProperties.toolTips[i].ReplaceWithCase(oldName, newName);
            }

            if (IsHeld && Holder == Player.LocalPlayer)
            {
                GrabbableObject.SetControlTipsForItem();
            }
        }

        /// <summary>
        /// Gets whether or not this item is currently being held.
        /// </summary>
        public bool IsHeld => GrabbableObject.isHeld;

        /// <summary>
        /// Gets the <see cref="Player"/> that is currently holding this <see cref="Item"/>. <see langword="null"/> if not held.
        /// </summary>
        public Player Holder => Player.Dictionary.TryGetValue(GrabbableObject.playerHeldBy, out Player p) ? p : null;

        private void Awake()
        {
            GrabbableObject = GetComponent<GrabbableObject>();
            ScanNodeProperties = GrabbableObject.gameObject.GetComponentInChildren<ScanNodeProperties>();

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

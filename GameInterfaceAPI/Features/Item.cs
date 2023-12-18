using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

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
        /// Gets the <see cref="Item"/>'s <see cref="Item">item properties</see>.
        /// These do not network, it is recommended to use the getters/setters on the <see cref="Item"/> itself.
        /// </summary>
        public global::Item ItemProperties => GrabbableObject.itemProperties;

        /// <summary>
        /// Gets the <see cref="Item"/>'s <see cref="ScanNodeProperties"/>.
        /// These do not network, it is recommended to use the getters/setters on the <see cref="Item"/> itself.
        /// </summary>
        public ScanNodeProperties ScanNodeProperties { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Item"/>'s name.
        /// </summary>
        /// <exception cref="Exception">Thrown when attempting to set item name from the client.</exception>
        public string Name
        {
            get
            {
                return ItemProperties.itemName;
            }
            set
            {
                if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
                {
                    throw new Exception("Tried to set item name on client.");
                }

                string current = ItemProperties.itemName.ToLower();

                CloneProperties();

                ItemProperties.itemName = value;
                OverrideTooltips(current, value.ToLower());

                ScanNodeProperties.headerText = value;

                SetGrabbableNameClientRpc(value);
            }
        }

        [ClientRpc]
        private void SetGrabbableNameClientRpc(string name)
        {
            string current = ItemProperties.itemName.ToLower();

            CloneProperties();

            ItemProperties.itemName = name;
            OverrideTooltips(current, name.ToLower());

            ScanNodeProperties.headerText = name;
        }

        private void OverrideTooltips(string oldName, string newName)
        {
            for (int i = 0; i < ItemProperties.toolTips.Length; i++)
            {
                ItemProperties.toolTips[i] = ItemProperties.toolTips[i].ReplaceWithCase(oldName, newName);
            }

            if (IsHeld && Holder == Player.LocalPlayer) GrabbableObject.SetControlTipsForItem();
        }

        /// <summary>
        /// Gets whether or not this item is currently being held.
        /// </summary>
        public bool IsHeld => GrabbableObject.isHeld;

        public bool IsScrap
        {
            get
            {
                return ItemProperties.isScrap;
            }
            set
            {
                if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
                {
                    throw new Exception("Tried to set item name on client.");
                }

                CloneProperties();

                ItemProperties.isScrap = value;

                SetIsScrapClientRpc(value);
            }
        }

        [ClientRpc]
        private void SetIsScrapClientRpc(bool isScrap)
        {
            CloneProperties();

            ItemProperties.isScrap = isScrap;
        }

        /// <summary>
        /// Gets the <see cref="Player"/> that is currently holding this <see cref="Item"/>. <see langword="null"/> if not held.
        /// </summary>
        public Player Holder => IsHeld ? Player.Dictionary.TryGetValue(GrabbableObject.playerHeldBy, out Player p) ? p : null : null;

        private void Awake()
        {
            GrabbableObject = GetComponent<GrabbableObject>();
            ScanNodeProperties = GrabbableObject.gameObject.GetComponentInChildren<ScanNodeProperties>();

            Dictionary.Add(GrabbableObject, this);
        }

        // All items have the same properties, so if we change it, we need to clone it.
        private bool hasNewProps = false;
        private void CloneProperties()
        {
            global::Item newProps = Instantiate(ItemProperties);

            // Don't want to destroy any that aren't custom
            if (hasNewProps) Destroy(ItemProperties);

            GrabbableObject.itemProperties = newProps;

            hasNewProps = true;
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

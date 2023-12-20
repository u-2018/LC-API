using GameNetcodeStuff;
using LC_API.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Gets whether or not this <see cref="Item"/> is currently being held.
        /// </summary>
        public bool IsHeld => GrabbableObject.isHeld;

        /// <summary>
        /// Gets whether or not this <see cref="Item"/> is two handed.
        /// </summary>
        public bool IsTwoHanded => ItemProperties.twoHanded;

        /// <summary>
        /// Gets the <see cref="Player"/> that is currently holding this <see cref="Item"/>. <see langword="null"/> if not held.
        /// </summary>
        public Player Holder => IsHeld ? Player.Dictionary.TryGetValue(GrabbableObject.playerHeldBy, out Player p) ? p : null : null;

        /// <summary>
        /// Gets or sets the <see cref="Item"/>'s name.
        /// </summary>
        /// <exception cref="NoAuthorityException">Thrown when attempting to set item name from the client.</exception>
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
                    throw new NoAuthorityException("Tried to set item name on client.");
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
        /// Gets or sets the position of this <see cref="Item"/>.
        /// </summary>
        /// <exception cref="NoAuthorityException">Thrown when attempting to set the item's position from the client.</exception>
        public Vector3 Position
        {
            get
            {
                return GrabbableObject.transform.position;
            }
            set
            {
                if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
                {
                    throw new NoAuthorityException("Tried to set item position on client.");
                }

                GrabbableObject.startFallingPosition = value;
                GrabbableObject.targetFloorPosition = value;
                GrabbableObject.transform.position = value;

                SetItemPositionClientRpc(value);
            }
        }

        [ClientRpc]
        private void SetItemPositionClientRpc(Vector3 pos)
        {
            GrabbableObject.startFallingPosition = pos;
            GrabbableObject.targetFloorPosition = pos;
            GrabbableObject.transform.position = pos;
        }

        /// <summary>
        /// Gets or sets the rotation of this <see cref="Item"/>. Does not sync
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return GrabbableObject.transform.rotation;
            }
            set
            {
                GrabbableObject.transform.rotation = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="Item"/>'s rotation and syncs it across all clients.
        /// </summary>
        /// <param name="rotation">The desired rotation.</param>
        /// <exception cref="NoAuthorityException">Thrown when attempting to sync rotation to other clients while not being the host.</exception>
        public void SetAndSyncRotation(Quaternion rotation)
        {
            if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            {
                throw new NoAuthorityException("Tried to sync item rotation from client.");
            }

            SetItemRotationClientRpc(rotation);
        }

        [ClientRpc]
        private void SetItemRotationClientRpc(Quaternion rotation)
        {
            Rotation = rotation;
        }

        /// <summary>
        /// Sets the scale of the <see cref="Item"/>. Does not sync.
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                return GrabbableObject.transform.localScale;
            }
            set
            {
                GrabbableObject.transform.localScale = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="Item"/>'s scale and syncs it across all clients.
        /// </summary>
        /// <param name="scale">The desired scale.</param>
        /// <exception cref="NoAuthorityException">Thrown when attempting to sync scale to other clients while not being the host.</exception>
        public void SetAndSyncScale(Vector3 scale)
        {
            if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            {
                throw new NoAuthorityException("Tried to sync item scale from client.");
            }

            SetItemScaleClientRpc(scale);
        }

        [ClientRpc]
        private void SetItemScaleClientRpc(Vector3 scale)
        {
            Scale = scale;
        }

        /// <summary>
        /// Gets or sets whether this <see cref="Item"/> should be considered scrap.
        /// </summary>
        /// <exception cref="NoAuthorityException">Thrown when attempting to set isScrap from the client.</exception>
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
                    throw new NoAuthorityException("Tried to set item name on client.");
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
        /// Gets or sets this <see cref="Item"/>'s scrap value.
        /// </summary>
        /// <exception cref="NoAuthorityException">Thrown when attempting to set scrap value from the client.</exception>
        public int ScrapValue
        {
            get
            {
                return GrabbableObject.scrapValue;
            }
            set
            {
                if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
                {
                    throw new NoAuthorityException("Tried to set scrap value on client.");
                }

                GrabbableObject.SetScrapValue(value);

                SetScrapValueClientRpc(value);
            }
        }

        [ClientRpc]
        private void SetScrapValueClientRpc(int scrapValue)
        {
            GrabbableObject.SetScrapValue(scrapValue);
        }

        /// <summary>
        /// Removes the <see cref="Item"/> from its current holder.
        /// </summary>
        /// <param name="position">The position to place the object after removing.</param>
        /// <param name="rotation">The rotation the object should have after removing.</param>
        public void RemoveFromHolder(Vector3 position = default, Quaternion rotation = default)
        {
            if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            {
                throw new NoAuthorityException("Tried to remove item from player on client.");
            }

            if (!IsHeld) return;

            NetworkObject.RemoveOwnership();

            Holder.Inventory.RemoveItem(this);

            RemoveFromHolderClientRpc();

            Position = position;
            Rotation = rotation;
        }

        [ClientRpc]
        private void RemoveFromHolderClientRpc()
        {
            if (!IsHeld) return;

            Holder.Inventory.RemoveItem(this);
        }

        /// <summary>
        /// Enables/disables the <see cref="Item"/>'s physics.
        /// </summary>
        /// <param name="enable"><see langword="true"/> to enable physics, <see langword="false" /> otherwise.</param>
        public void EnablePhysics(bool enable) => GrabbableObject.EnablePhysics(enable);

        /// <summary>
        /// Enables/disables the <see cref="Item"/>'s meshes.
        /// </summary>
        /// <param name="enable"><see langword="true"/> to enable meshes, <see langword="false" /> otherwise.</param>
        public void EnableMeshes(bool enable) => GrabbableObject.EnableItemMeshes(enable);

        /// <summary>
        /// Start the <see cref="Item"/> falling to the ground.
        /// </summary>
        /// <param name="randomizePosition">Whether or not to add some randomness to the position.</param>
        public void FallToGround(bool randomizePosition = false)
        {
            GrabbableObject.FallToGround(randomizePosition);
        }

        /// <summary>
        /// Pockets the <see cref="Item"/> by disabling its meshes. Plays pocket sound effects.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="Item"/> was able to be pocketed, <see langword="false" /> otherwise.</returns>
        public bool PocketItem()
        {
            // We can only pocket items that are currently being held in a player's hand. Two handed
            // objects cannot be pocketed.
            if (!IsHeld || Holder.HeldItem != this || IsTwoHanded) return false;

            GrabbableObject.PocketItem();

            return true;
        }

        /// <summary>
        /// Gives this <see cref="Item"/> to the specific player. Deleting it from another <see cref="Player"/>'s inventory, if necessary.
        /// </summary>
        /// <param name="player">The player to give the item to.</param>
        /// <param name="switchTo">Whether or not to switch to the item. Forced for 2 handed items.</param>
        /// <returns><see langword="true"/> if the player had an open slot to add the item to, <see langword="flase"/> otherwise.</returns>
        public bool GiveTo(Player player, bool switchTo = true)
        {
            if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            {
                throw new NoAuthorityException("Tried to give item to player on client.");
            }

            return player.Inventory.TryAddItem(this, switchTo);
        }

        /// <summary>
        /// Initializes the <see cref="Item"/> with base game scrap values.
        /// </summary>
        public void InitializeScrap()
        {
            InitializeScrap((int)(RoundManager.Instance.AnomalyRandom.Next(ItemProperties.minValue, ItemProperties.maxValue) * RoundManager.Instance.scrapValueMultiplier));
        }

        /// <summary>
        /// Initializes the <see cref="Item"/> with a specific scrap value.
        /// </summary>
        /// <param name="scrapValue">The desired scrap value.</param>
        /// <exception cref="NoAuthorityException">Thrown when trying to initialize scrap from the client.</exception>
        public void InitializeScrap(int scrapValue)
        {
            if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            {
                throw new NoAuthorityException("Tried to initialize scrap on client.");
            }

            ScrapValue = scrapValue;

            InitializeScrapClientRpc();
        }

        [ClientRpc]
        private void InitializeScrapClientRpc()
        {
            if (ItemProperties.meshVariants.Length != 0)
            {
                GrabbableObject.gameObject.GetComponent<MeshFilter>().mesh = ItemProperties.meshVariants[RoundManager.Instance.ScrapValuesRandom.Next(ItemProperties.meshVariants.Length)];
            }

            if (ItemProperties.materialVariants.Length != 0)
            {
                GrabbableObject.gameObject.GetComponent<MeshRenderer>().sharedMaterial = ItemProperties.materialVariants[RoundManager.Instance.ScrapValuesRandom.Next(ItemProperties.materialVariants.Length)];
            }
        }

        /// <summary>
        /// Creates and spawns an <see cref="Item"/> in the world.
        /// </summary>
        /// <param name="itemName">The item's name. Uses a simple Contains check to see if the provided item name is contained in the actual item's name. Case insensitive.</param>
        /// <param name="position">The position to spawn at.</param>
        /// <param name="rotation">The rotation to spawn at.</param>
        /// <returns>A new <see cref="Item"/>, or <see langword="null"/> if the provided item name is not found.</returns>
        /// <exception cref="NoAuthorityException">Thrown when trying to spawn an <see cref="Item"/> on the client.</exception>
        public static Item CreateAndSpawnItem(string itemName, Vector3 position = default, Quaternion rotation = default)
        {
            if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            {
                throw new NoAuthorityException("Tried to create and spawn item on client.");
            }

            string name = itemName.ToLower();

            GameObject go = StartOfRound.Instance.allItemsList.itemsList.FirstOrDefault(i => i.itemName.ToLower().Contains(name))?.spawnPrefab;
            if (go != null)
            {
                GameObject instantiated = Instantiate(go, position, rotation);

                instantiated.GetComponent<NetworkObject>().Spawn();

                return instantiated.GetComponent<Item>();
            }

            return null;
        }

        /// <summary>
        /// Creates an <see cref="Item"/> and gives it to a specific <see cref="Player"/>.
        /// </summary>
        /// <param name="itemName">The item's name. Uses a simple Contains check to see if the provided item name is contained in the actual item's name. Case insensitive.</param>
        /// <param name="player">The <see cref="Player"/> to give the <see cref="Item"/> to.</param>
        /// <param name="switchTo">Whether or not to switch to the item. Forced for 2 handed items.</param>
        /// <returns>A new <see cref="Item"/>, or <see langword="null"/> if the provided item name is not found.</returns>
        /// <exception cref="NoAuthorityException">Thrown when trying to spawn an <see cref="Item"/> on the client.</exception>
        public static Item CreateAndGiveItem(string itemName, Player player, bool switchTo = true)
        {
            if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            {
                throw new NoAuthorityException("Tried to create and give item on client.");
            }

            string name = itemName.ToLower();

            GameObject go = StartOfRound.Instance.allItemsList.itemsList.FirstOrDefault(i => i.itemName.ToLower().Contains(name))?.spawnPrefab;
            if (go != null)
            {
                GameObject instantiated = Instantiate(go, Vector3.zero, default);

                instantiated.GetComponent<NetworkObject>().Spawn();

                Item item = instantiated.GetComponent<Item>();

                item.GiveTo(player);

                return item;
            }

            return null;
        }

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

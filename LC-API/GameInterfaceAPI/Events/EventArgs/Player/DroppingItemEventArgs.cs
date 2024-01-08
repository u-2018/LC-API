using Unity.Netcode;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    public class DroppingItemEventArgs : System.EventArgs
    {
        public Features.Player Player { get; }

        public Features.Item Item { get; }

        public bool Placing { get; }

        public Vector3 TargetPosition { get; set; }

        public int FloorYRotation { get; set; }

        public NetworkObject ParentObjectTo { get; set; }

        public bool MatchRotationOfParent { get; set; }

        public bool DroppedInShip { get; set; }

        public bool IsAllowed { get; set; } = true;

        public DroppingItemEventArgs(Features.Player player, Features.Item item, bool placeObject, Vector3 targetPosition, 
            int floorYRotation, NetworkObject parentObjectTo, bool matchRotationOfParent, bool droppedInShip)
        {
            Player = player;
            Item = item;
            Placing = placeObject;
            TargetPosition = targetPosition;
            FloorYRotation = floorYRotation;
            ParentObjectTo = parentObjectTo;
            MatchRotationOfParent = matchRotationOfParent;
            DroppedInShip = droppedInShip;
        }
    }
}

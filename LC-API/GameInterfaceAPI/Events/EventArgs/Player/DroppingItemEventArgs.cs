namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    public class DroppingItemEventArgs : System.EventArgs
    {
        public Features.Player Player { get; }

        public Features.Item Item { get; }

        public bool PlaceObject { get; set; }

        public bool IsAllowed { get; set; } = true;

        public DroppingItemEventArgs(Features.Player player, Features.Item item, bool placeObject)
        {
            Player = player;
            Item = item;
            PlaceObject = placeObject;
        }
    }
}

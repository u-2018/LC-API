using System.Collections.Generic;

namespace LC_API.GameInterfaceAPI.Events.Cache
{
    internal static class Player
    {
        internal static List<ulong> ConnectedPlayers { get; private set; } = new List<ulong>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI.Events.Cache
{
    internal static class Player
    {
        internal static List<ulong> ConnectedPlayers { get; private set; } = new List<ulong>();
    }
}

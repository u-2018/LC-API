using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    /// <summary>
    /// Contains all the information after a <see cref="Features.Player"/> joined the server. Including the host.
    /// </summary>
    public class JoinedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the joined player.
        /// </summary>
        public Features.Player Player { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" /></param>
        public JoinedEventArgs(Features.Player player)
        {
            Player = player;
        }
    }
}

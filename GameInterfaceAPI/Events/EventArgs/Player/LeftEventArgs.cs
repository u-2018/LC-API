using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    /// <summary>
    /// Contains all the information right before a <see cref="Features.Player"/> leaves the server. Including the local client.
    /// </summary>
    public class LeftEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the player that is leaving.
        /// </summary>
        public Features.Player Player { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" /></param>
        public LeftEventArgs(Features.Player player)
        {
            Player = player;
        }
    }
}

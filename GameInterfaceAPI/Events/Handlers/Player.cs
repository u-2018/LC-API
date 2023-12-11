using GameNetcodeStuff;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LC_API.GameInterfaceAPI.Events.Events;

namespace LC_API.GameInterfaceAPI.Events.Handlers
{
    /// <summary>
    /// Provides event handlers relating to Players.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Invoked after a <see cref="Features.Player"/> joined the server, including the host.
        /// </summary>
        public static event CustomEventHandler<PlayerJoinedEventArgs> Joined;

        /// <summary>
        /// Invoked right before a <see cref="Features.Player"/> leaves the server, including the local client.
        /// </summary>
        public static event CustomEventHandler<PlayerLeftEventArgs> Left;

        /// <summary>
        /// Called after a <see cref="Features.Player"/> joined the server, including the host.
        /// </summary>
        /// <param name="ev">The <see cref="PlayerJoinedEventArgs"/> event arguments.</param>
        public static void OnJoined(PlayerJoinedEventArgs ev) => Joined.InvokeSafely(ev);

        /// <summary>
        /// Called right before a <see cref="Features.Player"/> leaves the server, including the local client.
        /// </summary>
        /// <param name="ev">The <see cref="PlayerLeftEventArgs"/> event arguments.</param>
        public static void OnLeft(PlayerLeftEventArgs ev) => Left.InvokeSafely(ev);
    }
}

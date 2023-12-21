using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
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
        public static event CustomEventHandler<JoinedEventArgs> Joined;

        /// <summary>
        /// Invoked right before a <see cref="Features.Player"/> leaves the server, including the local client.
        /// </summary>
        public static event CustomEventHandler<LeftEventArgs> Left;

        /// <summary>
        /// Invoked before a <see cref="Features.Player"/> is hurt.
        /// </summary>
        public static event CustomEventHandler<HurtingEventArgs> Hurting;

        /// <summary>
        /// Invoked before a <see cref="Features.Player"/> dies.
        /// </summary>
        public static event CustomEventHandler<DyingEventArgs> Dying;

        /// <summary>
        /// Called after a <see cref="Features.Player"/> joined the server, including the host.
        /// </summary>
        /// <param name="ev">The <see cref="JoinedEventArgs"/> event arguments.</param>
        public static void OnJoined(JoinedEventArgs ev) => Joined.InvokeSafely(ev);

        /// <summary>
        /// Called right before a <see cref="Features.Player"/> leaves the server, including the local client.
        /// </summary>
        /// <param name="ev">The <see cref="LeftEventArgs"/> event arguments.</param>
        public static void OnLeft(LeftEventArgs ev) => Left.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="Features.Player"/> is hurt.
        /// </summary>
        /// <param name="ev">The <see cref="HurtingEventArgs"/> event arguments.</param>
        public static void OnHurting(HurtingEventArgs ev) => Hurting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="Features.Player"/> dies.
        /// </summary>
        /// <param name="ev">The <see cref="DyingEventArgs"/> event arguments.</param>
        public static void OnDying(DyingEventArgs ev) => Dying.InvokeSafely(ev);
    }
}

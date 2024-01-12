namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    /// <summary>
    /// Contains all the information before a <see cref="Features.Player"/> picks up an <see cref="Features.Item"/>.
    /// </summary>
    public class GrabbingItemEventArgs : System.EventArgs
    {
        /// <summary>
        /// The <see cref="Features.Player"/> picking up the <see cref="Features.Item"/>.
        /// </summary>
        public Features.Player Player { get; }

        /// <summary>
        /// The <see cref="Features.Item"/> being picked up.
        /// </summary>
        public Features.Item Item { get; }

        /// <summary>
        /// Gets or sets whether this is allowed to occur.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrabbingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" /></param>
        /// <param name="item"><inheritdoc cref="Item" /></param>
        public GrabbingItemEventArgs(Features.Player player, Features.Item item)
        {
            Player = player;
            Item = item;
        }
    }
}

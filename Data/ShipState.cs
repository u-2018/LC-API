using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.Data
{
    /// <summary>
    /// Describes the state the company ship is currently in.
    /// </summary>
    public enum ShipState
    {
        /// <summary>
        /// The ship is currently in orbit.
        /// <para>Players are likely either goofing off, AFK, or picking the next moon to visit.</para>
        /// </summary>
        InOrbit,
        /// <summary>
        /// The ship is currently on a moon.
        /// <para>Players are likely getting torn limb from limb, getting , or witnessing horrors target their fellow man.</para>
        /// </summary>
        OnMoon,
        /// <summary>
        /// The ship has been told to leave and it is currently doing so.
        /// <para>This could either be caused by ghosts of players' pasts (dead players) telling the autopilot to do so, or a living player pulling the lever. Either way, the ship is taking off.</para>
        /// </summary>
        LeavingMoon,
    }
}

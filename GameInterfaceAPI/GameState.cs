using GameNetcodeStuff;
using LC_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI
{
    public class GameState
    public static class GameState
    {
        public delegate void GenericGameEventDelegate();

        public static int AlivePlayerCount;
        public static ShipStateEnum ShipState;
        public static GenericGameEventDelegate PlayerDied = GSPlayerDied;
        public static GenericGameEventDelegate LandOnMoon = GSLandOnMood;
        public static GenericGameEventDelegate WentIntoOrbit = GSGoIntoOrbit;
        public static GenericGameEventDelegate ShipStartedLeaving = GSStartLeavingMoon;

        public static void GSUpdate()
        {
            if (StartOfRound.Instance != null)
            {
                if (StartOfRound.Instance.shipHasLanded)
                {
                    if (ShipState != ShipStateEnum.OnMoon)
                    {
                        ShipState = ShipStateEnum.OnMoon;
                        LandOnMoon();
                    }
                }
                if (StartOfRound.Instance.inShipPhase)
                {
                    if (ShipState != ShipStateEnum.InOrbit)
                    {
                        ShipState = ShipStateEnum.InOrbit;
                        WentIntoOrbit();
                    }
                }
                if (StartOfRound.Instance.shipIsLeaving)
                {
                    if (ShipState != ShipStateEnum.LeavingMoon)
                    {
                        ShipState = ShipStateEnum.LeavingMoon;
                        ShipStartedLeaving();
                    }
                }
                int aliveP = AlivePlayerCount;
                if (aliveP < StartOfRound.Instance.livingPlayers)
                {
                    PlayerDied();
                }
                AlivePlayerCount = StartOfRound.Instance.livingPlayers;
            }
        }

        private static void GSPlayerDied()
        {

        }

        private static void GSLandOnMood()
        {

        }

        private static void GSGoIntoOrbit()
        {

        }

        private static void GSStartLeavingMoon()
        {

        }
    }
}

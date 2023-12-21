using BepInEx;
using BepInEx.Bootstrap;
using LC_API.GameInterfaceAPI;
using LC_API.ServerAPI;
using System.Collections.Generic;

namespace LC_API
{
    internal static class CheatDatabase
    {
        const string DAT_CD_BROADCAST = "LC_API_CD_Broadcast";
        const string SIG_REQ_GUID = "LC_API_ReqGUID";
        const string SIG_SEND_MODS = "LC_APISendMods";

        private static Dictionary<string, PluginInfo> PluginsLoaded = new Dictionary<string, PluginInfo>();

        public static void RunLocalCheatDetector()
        {
            PluginsLoaded = Chainloader.PluginInfos;

            foreach (PluginInfo info in PluginsLoaded.Values)
            {
                switch (info.Metadata.GUID)
                {
                    case "mikes.lethalcompany.mikestweaks":
                    case "mom.llama.enhancer":
                    case "Posiedon.GameMaster":
                    case "LethalCompanyScalingMaster":
                    case "verity.amberalert":
                        ServerAPI.ModdedServer.SetServerModdedOnly();
                        break;
                }
            }

        }

        public static void OtherPlayerCheatDetector()
        {
            Plugin.Log.LogWarning("Asking all other players for their mod list..");
            GameTips.ShowTip("Mod List:", "Asking all other players for installed mods..");
            GameTips.ShowTip("Mod List:", "Check the logs for more detailed results.\n<size=13>(Note that if someone doesnt show up on the list, they may not have LC_API installed)</size>");
            Networking.Broadcast(DAT_CD_BROADCAST, SIG_REQ_GUID);
        }

        internal static void CDNetGetString(string data, string signature)
        {
            if (data == DAT_CD_BROADCAST && signature == SIG_REQ_GUID)
            {
                string mods = "";
                foreach (PluginInfo info in PluginsLoaded.Values)
                {
                    mods += "\n" + info.Metadata.GUID;
                }
                Networking.Broadcast(GameNetworkManager.Instance.localPlayerController.playerUsername + " responded with these mods:" + mods, SIG_SEND_MODS);
            }

            if (signature == SIG_SEND_MODS)
            {
                GameTips.ShowTip("Mod List:", data);
                Plugin.Log.LogWarning(data);
            }
        }
    }
}

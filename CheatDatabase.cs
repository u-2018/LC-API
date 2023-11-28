using BepInEx;
using BepInEx.Bootstrap;
using LC_API.ServerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                switch(info.Metadata.GUID)
                {
                    case "mikes.lethalcompany.mikestweaks":
                    case "mom.llama.enhancer":
                    case "Posiedon.GameMaster":
                    case "LethalCompanyScalingMaster":
                        ServerAPI.ModdedServer.SetServerModdedOnly();
                        break;
                }
            }
        }

        public static void OtherPlayerCheatDetector()
        {
            Plugin.Log.LogWarning("Asking all other players for their mod list..");
            HUDManager.Instance.chatText.text += "\n" + "<color=white>Grabbing all connected users mod list\nCheck the log for results!!</color>";
            Networking.Broadcast(DAT_CD_BROADCAST, SIG_REQ_GUID);
        }

        internal static void RequestModList(string data, string signature)
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
                Plugin.Log.LogWarning(data);
            }
        }
    }
}

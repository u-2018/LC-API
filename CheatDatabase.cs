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
        private static Dictionary<string, PluginInfo> PluginsLoaded = new Dictionary<string, PluginInfo>();

        public static void RunLocalCheatDetector()
        {
            PluginsLoaded = Chainloader.PluginInfos;

            foreach (PluginInfo info in PluginsLoaded.Values)
            {
                if (info.Metadata.GUID == "mikes.lethalcompany.mikestweaks")
                {
                    ServerAPI.ModdedServer.SetServerModdedOnly();
                }
                if (info.Metadata.GUID == "mom.llama.enhancer")
                {
                    ServerAPI.ModdedServer.SetServerModdedOnly();
                }
                if (info.Metadata.GUID == "Posiedon.GameMaster")
                {
                    
                    ServerAPI.ModdedServer.SetServerModdedOnly();
                }
                if (info.Metadata.GUID == "LethalCompanyScalingMaster")
                {
                    ServerAPI.ModdedServer.SetServerModdedOnly();
                }
            }
        }

        public static void OtherPlayerCheatDetector()
        {
            Plugin.Log.LogWarning("Asking all other players for their mod list..");
            HUDManager.Instance.chatText.text += "\n" + "<color=white>Grabbing all connected users mod list\nCheck the log for results!!</color>";
            Networking.Broadcast("LC_API_CD_Broadcast", "LC_API_ReqGUID");
        }

        internal static void RequestModList(string data, string signature)
        {
            if (data == "LC_API_CD_Broadcast" & signature == "LC_API_ReqGUID")
            {
                string mods = "";
                foreach (PluginInfo info in PluginsLoaded.Values)
                {
                    mods += "\n" + info.Metadata.GUID;
                }
                Networking.Broadcast(GameNetworkManager.Instance.localPlayerController.playerUsername + " responded with these mods:" + mods, "LC_APISendMods");
            }

            if (signature == "LC_APISendMods")
            {
                Plugin.Log.LogWarning(data);
            }
        }
    }
}

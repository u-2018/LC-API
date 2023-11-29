using BepInEx;
using BepInEx.Bootstrap;
using LC_API.GameInterfaceAPI;
using LC_API.ServerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LC_API
{
    public class CheatDatabase
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
            //messageQue.Add("Asking all other players for installed mods...\nCheck your logs for detailed results.");
            GameTips.ShowTip("Mod List:", "Asking all other players for installed mods..");
            GameTips.ShowTip("Mod List:", "Check the logs for more detailed results.\n<size=13>(Note that if someone doesnt show up on the list, they may not have LC_API installed)</size>");
            Networking.Broadcast("LC_API_CD_Broadcast", "LC_API_ReqGUID");
        }

        internal static void CDNetGetString(string data, string signature)
        {
            if (data == "LC_API_CD_Broadcast" & signature == "LC_API_ReqGUID")
            {
                string mods = "";
                foreach (PluginInfo info in PluginsLoaded.Values)
                {
                    mods += "\n" + info.Metadata.GUID;
                }
                Networking.Broadcast(GameNetworkManager.Instance.localPlayerController.playerUsername + " has:" + mods, "LC_APISendMods");
            }

            if (signature == "LC_APISendMods")
            {
                GameTips.ShowTip("Mod List:", data);
                Plugin.Log.LogWarning(data);
            }
        }
    }
}

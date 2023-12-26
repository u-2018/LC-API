using BepInEx;
using BepInEx.Bootstrap;
using LC_API.GameInterfaceAPI;
using LC_API.GameInterfaceAPI.Features;
using LC_API.Networking;
using LC_API.ServerAPI;
using System.Collections.Generic;

namespace LC_API
{
    internal static class CheatDatabase
    {
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
            Network.Broadcast(SIG_REQ_GUID);
        }

        [NetworkMessage(SIG_SEND_MODS)]
        internal static void ReceivedModListHandler(ulong senderId, List<string> mods)
        {
            Player player = Player.Get(senderId);
            string data = $"{player.Username} responded with these mods:\n{string.Join("\n", mods)}";
            GameTips.ShowTip("Mod List:", data);
            Plugin.Log.LogWarning(data);
        }

        [NetworkMessage(SIG_REQ_GUID)]
        internal static void ReceivedModListHandler(ulong senderId)
        {
            List<string> mods = new List<string>();
            foreach (PluginInfo info in PluginsLoaded.Values)
            {
                mods.Add(info.Metadata.GUID);
            }

            Network.Broadcast(SIG_SEND_MODS, mods);
        }
    }
}

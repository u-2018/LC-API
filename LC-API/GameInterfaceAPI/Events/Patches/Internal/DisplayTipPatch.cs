using HarmonyLib;
using LC_API.GameInterfaceAPI.Features;

namespace LC_API.GameInterfaceAPI.Events.Patches.Internal
{
    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.DisplayTip))]
    class DisplayTipPatch
    {
        // Normally we wouldn't prefix return false, however, we need all uses of `DisplayTip` to go through
        // our system in order to not cause improper timing. All uses of base game's `DisplayTip` will bypass queue
        // and immediately be shown, but preserving the currently showing tip, if there is one, and continuing it after
        // the tip is complete.
        private static bool Prefix(string headerText, string bodyText, bool isWarning, bool useSave, string prefsKey)
        {
            Features.Player player = Features.Player.LocalPlayer;

            if (player == null) return true;

            Tip tip = new Tip(headerText, bodyText, 5, 0, isWarning, useSave, prefsKey, 0);

            if (player.CurrentTip != null && player.CurrentTip.TimeLeft >= 1.5f)
            {
                // Ensures the current tip will continue afterwards
                player.CurrentTip.TipId = int.MinValue;
                player.TipQueue.Insert(0, player.CurrentTip);
            }

            player.CurrentTip = tip;

            HUDManager.Instance.tipsPanelAnimator.speed = 1;
            HUDManager.Instance.tipsPanelAnimator.ResetTrigger("TriggerHint");

            Features.Player.DisplayTip(tip.Header, tip.Message, isWarning, useSave, prefsKey);

            return false;
        }
    }
}

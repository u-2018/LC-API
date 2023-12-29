using System;

namespace LC_API.GameInterfaceAPI.Events
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class EventExtensions
    {
        public static void InvokeSafely<T>(this Events.CustomEventHandler<T> ev, T arg)
            where T : System.EventArgs
        {
            if (ev == null)
                return;

            foreach (Events.CustomEventHandler<T> handler in ev.GetInvocationList())
            {
                try
                {
                    handler(arg);
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(ex);
                }
            }
        }

        public static void InvokeSafely(this Events.CustomEventHandler ev)
        {
            if (ev == null)
                return;

            foreach (Events.CustomEventHandler handler in ev.GetInvocationList())
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(ex);
                }
            }
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
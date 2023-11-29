using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.Extensions
{
    /// <summary>
    /// Extension methods for "Delegates". Basically makes working with 'callable' (invokable) things easier.
    /// </summary>
    public static class DelegateExtensions
    {
        private static readonly PropertyInfo PluginGetLogger = AccessTools.Property(typeof(BaseUnityPlugin), "Logger");

        /// <summary>
        /// Call an action (usually a list of hook callbacks) in a way that logs errors instead of throwing them. This avoids only executing <i>some</i> callbacks if one errors out.
        /// </summary>
        public static void InvokeActionSafe(this Action action)
        {
            if (action == null) return;

            foreach (Delegate invoker in action.GetInvocationList())
            {
                try
                {
                    Action call = (Action)invoker;
                    call();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError("Exception while invoking hook callback!");

                    string asmName = invoker.GetMethodInfo().DeclaringType.Assembly.FullName;
                    PluginInfo plugin = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.FirstOrDefault(pi => pi.Instance.GetType().Assembly.FullName == asmName);
                    if (plugin == null)
                    {
                        Plugin.Log.LogError(ex.ToString());
                        return;
                    }
                    
                    var pluginsLogger = (ManualLogSource)PluginGetLogger.GetValue(plugin.Instance);
                    pluginsLogger.LogError(ex.ToString());
                }
            }
        }


        /// <summary>
        /// Call an action (usually a list of hook callbacks) in a way that logs errors instead of throwing them. This avoids only executing <i>some</i> callbacks if one errors out.
        /// </summary>
        public static void InvokeActionSafe<T>(this Action<T> action, T param)
        {
            if (action == null) return;

            foreach (Delegate invoker in action.GetInvocationList())
            {
                try
                {
                    Action<T> call = (Action<T>)invoker;
                    call(param);
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError("Exception while invoking hook callback!");

                    string asmName = invoker.GetMethodInfo().DeclaringType.Assembly.FullName;
                    PluginInfo plugin = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.FirstOrDefault(pi => pi.Instance.GetType().Assembly.FullName == asmName);
                    if (plugin == null)
                    {
                        Plugin.Log.LogError(ex.ToString());
                        return;
                    }

                    var pluginsLogger = (ManualLogSource)PluginGetLogger.GetValue(plugin.Instance);
                    pluginsLogger.LogError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Call an action (usually a list of hook callbacks) in a way that logs errors instead of throwing them. This avoids only executing <i>some</i> callbacks if one errors out.
        /// </summary>
        public static void InvokeActionSafe<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
        {
            if (action == null) return;

            foreach (Delegate invoker in action.GetInvocationList())
            {
                try
                {
                    Action<T1, T2> call = (Action<T1, T2>)invoker;
                    call(param1, param2);
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError("Exception while invoking hook callback!");

                    string asmName = invoker.GetMethodInfo().DeclaringType.Assembly.FullName;
                    PluginInfo plugin = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.FirstOrDefault(pi => pi.Instance.GetType().Assembly.FullName == asmName);
                    if (plugin == null)
                    {
                        Plugin.Log.LogError(ex.ToString());
                        return;
                    }

                    var pluginsLogger = (ManualLogSource)PluginGetLogger.GetValue(plugin.Instance);
                    pluginsLogger.LogError(ex.ToString());
                }
            }
        }


        // internal because it uses the slow DynamicInvoke, and almost any event delegate can be an Action(<T>/<T1, T2>/etc), at least in modding.
        internal static void InvokeParameterlessDelegate<T>(this T paramlessDelegate) where T : Delegate
        {
            if (paramlessDelegate == null) return;

            foreach (Delegate invoker in paramlessDelegate.GetInvocationList())
            {
                try
                {
                    T call = (T)invoker;
                    call.DynamicInvoke();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError("Exception while invoking hook callback!");

                    string asmName = invoker.GetMethodInfo().DeclaringType.Assembly.FullName;
                    PluginInfo plugin = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.FirstOrDefault(pi => pi.Instance.GetType().Assembly.FullName == asmName);
                    if (plugin == null)
                    {
                        Plugin.Log.LogError(ex.ToString());
                        return;
                    }

                    var pluginsLogger = (ManualLogSource)PluginGetLogger.GetValue(plugin.Instance);
                    pluginsLogger.LogError(ex.ToString());
                }
            }
        }
    }
}

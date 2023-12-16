using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI.Events
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Events
    {
        public delegate void CustomEventHandler<TEventArgs>(TEventArgs ev)
            where TEventArgs : System.EventArgs;

        public delegate void CustomEventHandler();

        internal static void Patch(Harmony harmony) 
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            var assembly = method.ReflectedType.Assembly;
            foreach (Type t in AccessTools.GetTypesFromAssembly(assembly))
            {
                if (t.Namespace.StartsWith("LC_API.GameInterfaceAPI.Events.Patches"))
                {
                    harmony.CreateClassProcessor(t).Patch();
                }
            }
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

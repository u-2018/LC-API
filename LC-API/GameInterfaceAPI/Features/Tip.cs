using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Features
{
    public class Tip : IComparable<Tip>
    {
        public string Header { get; }

        public string Message { get; }

        public float Duration { get; }

        public float TimeLeft { get; internal set; }

        public int Priority { get; } = 0;

        internal int TipId { get; set; }

        public Tip(string header, string message, float duration, int priority, int tipId)
        {
            Header = header;
            Message = message;
            Duration = duration;
            TimeLeft = duration;

            Priority = priority;

            TipId = tipId;
        }

        public int CompareTo(Tip other)
        {
            int diff = other.Priority - Priority;

            if (diff < 0) return -1;

            if (diff > 0) return 1;

            return TipId - other.TipId;
        }
    }
}

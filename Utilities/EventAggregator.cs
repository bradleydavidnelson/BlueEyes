using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Utilities
{
    public static class EventAggregator
    {
        public static void BroadCast(string message)
        {
            if (OnMessageTransmitted != null)
                OnMessageTransmitted(message);
        }

        public static Action<string> OnMessageTransmitted;
    }
}

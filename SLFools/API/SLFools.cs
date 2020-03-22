using System.Collections.Generic;

namespace SLFools.API
{
    public static class SLFools
    {
        public static List<string> GetRandomCassieLines()
        {
            return EventHandlers.cassieAnnounce;
        }

        public static bool GetEnabedStatus()
        {
            return Plugin.isEnabled;
        }
    }
}

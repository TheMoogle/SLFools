using EXILED;
using MEC;

namespace SLFools
{
    public class Plugin : EXILED.Plugin
    {
        EventHandlers Eventhandler;
        internal static bool scalePlayers;
        internal static bool grenadeRandomSpawn;
        internal static bool randomAnnouncements;
        internal bool isEnabled;

        public override void OnEnable()
        {
            LoadConfigs();

            if (!isEnabled) return;

            Eventhandler = new EventHandlers(this);
            Events.RoundStartEvent += Eventhandler.OnRoundStart;
            Events.RoundEndEvent += Eventhandler.OnRoundEnd;
            Events.PlayerSpawnEvent += Eventhandler.OnPlayerSpawn;
            Events.WaitingForPlayersEvent += Eventhandler.OnWaitingforPlayers;
        }

        public override void OnDisable()
        {
            foreach (CoroutineHandle handle in Eventhandler.Coroutines)
                Timing.KillCoroutines(handle);
            Events.WaitingForPlayersEvent -= Eventhandler.OnWaitingforPlayers;
            Events.PlayerSpawnEvent -= Eventhandler.OnPlayerSpawn;
            Events.RoundEndEvent -= Eventhandler.OnRoundEnd;
            Events.RoundStartEvent -= Eventhandler.OnRoundStart;
            
            Eventhandler = null;
        }

        private void LoadConfigs()
        {
            isEnabled = Config.GetBool("slf_enabled", true);
            scalePlayers = Config.GetBool("slf_scale", true);
            grenadeRandomSpawn = Config.GetBool("slf_rgrenade", false);
            randomAnnouncements = Config.GetBool("slf_ranann", true);
        }

        public override void OnReload()
        {

        }

        public override string getName { get; } = "SLFools";

    }
}

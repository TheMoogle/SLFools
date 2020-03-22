using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using EXILED.Extensions;
using MEC;
using Grenades;
using UnityEngine;
using Object = UnityEngine.Object;
using Mirror;

namespace SLFools
{
    public class EventHandlers
    {
        private Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;
        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
        private System.Random rand = new System.Random();

        internal static List<string> cassieAnnounce = new List<string>()
        {
         "THE ALPHA WARHEAD HAS BEEN DESTROYED",
         "RED KING IS DEAD",
         "AWAITINGRECONTAINMENT SCP 4 20 J",
         "ARREST CLASSD",
         "MTFUNIT IS CORRUPTED",
         "DEFENSE AGAINST THE DARK FORCE",
         "THE G O C XMAS_HASENTERED MTFUNIT",
         "XMAS_BOUNCYBALLS",
         "MEMORY AGENT IS IN FACILITY",
         "YOU ALL FAILED",
         "FEMUR BREAKER DOES NOT WORK",
         "SOME LABORATORY THIS IS",
         "FACILITY POWER LEVEL IS OVER 9000",
         "L S HAS SCP 2 6 8",
         "THE CHAOSINSURGENCY IS NOW THE G O C",
         "THE GROUND IS NOW AN ANOMALY",
         "KILL ALL SCIENTISTS",
         "RUN FOR YOUR ESCAPE",
         "BELIEVE IN THE HELICOPTER",
         "INTERCOM IS READY",
         "GET A REAL G F TODAY",
         "TUESDAY IS THE WORST DAY",
         "I NEED YOUR CREDIT CARD INFORMATION",
         "INSTALLING O5 COMMAND IN FACILITY",
         "THE MILITARY ARE DEAD",
         "EXECUTIVE DEACTIVATED",
         "SERPENTS HAND IS ON SCP 4 20 J",
         "VIRUS INFECTION HAS TURNED ON THE ALPHA WARHEAD",
         "WELCOME TO WEDNESDAY",
         "ATTENTION I HAVE ACTIVATED THE WAY ON SURFACE",
         "MONDAY IS NOT REAL",
         "DATA ANALYSIS COMPLETED . . INITIALIZING SURFACE WARHEAD",
         "INTERCOM IS UNABLE TO BE PAUSED",
         "THE PLAGUE IS REAL",
         "SITE SECURITY IS DEAD",
         "PLEASE RESPAWN",
         "REPEAT RADIATION QUERY",
         "KILL RED KING",
         "YEAR IS OVER",
         "DIGITAL DEVICE IS KILL",
         "A B C D E F G H I J K",
         "SIGMA WOOD",
         "WEAPONS ARE NOT WELCOME",
         "IT IS WEDNESDAY MY FACILITY PERSONNEL"
        };


        public void OnPlayerSpawn(PlayerSpawnEvent ev)
        {
            if (!Plugin.isEnabled) return;

            if (Plugin.scalePlayers)
                if (ev.Player.characterClassManager.CurClass != RoleType.Spectator)
                    SetPlayerScale(ev.Player.gameObject, UnityEngine.Random.Range(0.6f, 1.1f));

        }

        // This is from AdminTools
        internal void SetPlayerScale(GameObject target, float x, float y, float z)
        {
            try
            {
                NetworkIdentity identity = target.GetComponent<NetworkIdentity>();

                target.transform.localScale = new Vector3(1 * x, 1 * y, 1 * z);

                ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage();
                destroyMessage.netId = identity.netId;

                foreach(GameObject player in PlayerManager.players)
                {
                    NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;

                    if (player != target)
                        playerCon.Send(destroyMessage, 0);

                    object[] parameters = new object[] { identity, playerCon };
                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Error: {e}");
            }
        }

        // Also From AdminTools
        internal void SetPlayerScale(GameObject target, float scale)
        {
            try
            {
                NetworkIdentity identity = target.GetComponent<NetworkIdentity>();


                target.transform.localScale = Vector3.one * scale;

                ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage();
                destroyMessage.netId = identity.netId;


                foreach (GameObject player in PlayerManager.players)
                {
                    if (player == target)
                        continue;

                    NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;

                    playerCon.Send(destroyMessage, 0);

                    object[] parameters = new object[] { identity, playerCon };
                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }

        public void OnRoundStart()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
            if (!Plugin.isEnabled) return;
            if (Plugin.grenadeRandomSpawn)
                Coroutines.Add(Timing.RunCoroutine(randomGrenadeTimer()));
            if (Plugin.randomAnnouncements)
                Coroutines.Add(Timing.RunCoroutine(randomAnnoucements()));
        }


        public void OnRoundEnd()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
        }


        public void OnPlayerLeave(PlayerLeaveEvent ev)
        {
            if (!Plugin.isEnabled) return;
        }

        public void OnWaitingforPlayers()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
            if (!Plugin.isEnabled) return;
        }

        public IEnumerator<float> randomAnnoucements()
        {
            if (Plugin.randomAnnouncements)
            {
                for (;;)
                {
                    yield return Timing.WaitForSeconds(rand.Next(60, 240));
                    int announcementid = rand.Next(0, cassieAnnounce.Count - 1);
                    Cassie.CassieMessage(cassieAnnounce[announcementid], true, false);
                }
            }
            else
                yield return Timing.WaitForSeconds(9999999f);
        }

        public IEnumerator<float> randomGrenadeTimer()
        {
            if (Plugin.grenadeRandomSpawn)
            {
                for (;;)
                {
                    yield return Timing.WaitForSeconds(rand.Next(60, 250));
                    foreach (ReferenceHub player in Player.GetHubs())
                    {
                        yield return Timing.WaitForSeconds(rand.Next(0, 5));
                        if (player.characterClassManager.CurClass != RoleType.Spectator)
                        {
                                GrenadeSettings grenade;
                                Vector3 spawnrand = new Vector3(UnityEngine.Random.Range(0f, 5f), UnityEngine.Random.Range(0f, 5f), UnityEngine.Random.Range(0f, 5f));
                                GrenadeManager gm = player.GetComponent<GrenadeManager>();
                                int grenadetype = rand.Next(0, 2);
                                if (grenadetype == 0)
                                {
                                    grenade = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFrag);
                                    Grenade component = Object.Instantiate(grenade.grenadeInstance).GetComponent<FragGrenade>();
                                    component.InitData(gm, Vector3.zero, Vector3.zero, 0f);
                                    NetworkServer.Spawn(component.gameObject);
                                }
                                else if (grenadetype == 1)
                                {
                                    grenade = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFlash);
                                    Grenade component = Object.Instantiate(grenade.grenadeInstance).GetComponent<FlashGrenade>();
                                    component.InitData(gm, Vector3.zero, Vector3.zero, 0f);
                                    NetworkServer.Spawn(component.gameObject);
                                }
                                else
                                {
                                    grenade = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.SCP018);
                                    Grenade component = Object.Instantiate(grenade.grenadeInstance).GetComponent<Scp018Grenade>();
                                    component.InitData(gm, spawnrand, spawnrand, 0f);
                                    NetworkServer.Spawn(component.gameObject);
                                }
                        }
                    }
                }
            } else
            {
                yield return Timing.WaitForSeconds(1000000f);
            }
        }


    }
}

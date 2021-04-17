using Exiled.API.Enums;
using Exiled.API.Features;
using Grenades;
using MEC;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lootorial.Classes;
using NPCS;
using Exiled.API.Extensions;
using Random = UnityEngine.Random;

namespace Lootorial.Methods
{
    public static class Methods
    {
        public static readonly System.Random Rand = new System.Random();
        private static readonly Vector3 InitialPosVec3 = new Vector3(0f, 0.5f, 0f);

        public static NPCS.Npc CreatePinhata(PinhataPosition pin)
        {
            if (Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.TryGetValue(pin, out GameObject value) && value != null)
            {
                return Player.Get(value).AsNPC();
            }

            var room = Map.Rooms.FirstOrDefault(x => x.Name == pin.Room);
            var pos = room.Transform.TransformPoint(pin.Position);
            var dir = room.Transform.TransformDirection(pin.Direction);
            var npc = NPCS.Methods.CreateNPC(pos, dir, pin.Scale, pin.Role, ItemType.None, pin.Name);

            Timing.CallDelayed(0.5f, () =>
            {
                npc.NPCPlayer.Health = pin.Health;
                npc.NPCPlayer.MaxHealth = (int)pin.Health;
                Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed[pin] = npc.gameObject;
                for (int i = 0; i < Lootorial.Instance.Config.MaxItemsPerPinhata + 1; i++)
                {
                    var item = Rand.Next(0, Lootorial.Instance.Config.DroppableItems.Count);
                    npc.NPCPlayer.AddItem(Lootorial.Instance.Config.DroppableItems.ElementAt(item));
                }
            });

            return npc;
        }

        public static NPCS.Npc SpawnPinhata(PinhataPosition pin)
        {
            var npc = NPCS.Methods.CreateNPC(pin.Position, pin.Direction, pin.Scale, pin.Role, ItemType.None, pin.Name);

            Timing.CallDelayed(0.5f, () =>
            {
                npc.NPCPlayer.Health = pin.Health;
                npc.NPCPlayer.MaxHealth = (int)pin.Health;
                for (int i = 0; i < Lootorial.Instance.Config.MaxItemsPerPinhata + 1; i++)
                {
                    var item = Rand.Next(0, Lootorial.Instance.Config.DroppableItems.Count);
                    npc.NPCPlayer.AddItem(Lootorial.Instance.Config.DroppableItems.ElementAt(item));
                }
            });

            return npc;
        }

        public static IEnumerator<float> ThrowItem(Pickup pickup, Vector3 dir)
        {
            yield return Timing.WaitUntilFalse(() => pickup != null && pickup.Rb == null);
            try
            {
                pickup.Rb.transform.Translate(InitialPosVec3, Space.Self);
                pickup.Rb.AddForce(dir * Lootorial.Instance.Config.ThrowForce, ForceMode.Impulse);
                Vector3 rand = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-100f, 1f)).normalized;
                pickup.Rb.angularVelocity = rand.normalized * Lootorial.Instance.Config.RandomSpinForce;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static IEnumerator<float> SpawnRandomlyEveryXseconds()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(Lootorial.Instance.Config.SpawnRandomlyPinhatas);
                var pinhata = Rand.Next(0, Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.Count);

                if (!Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.ElementAt(pinhata).Value)
                {
                    if (Rand.Next(100) < Lootorial.Instance.Config.SpawnChance)
                    {
                        var pin = Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.ElementAt(pinhata).Key;
                        Map.Broadcast(10, Lootorial.Instance.Config.PinhataBroadcast.Replace("%room", pin.Room));
                        CreatePinhata(pin);
                    }
                }
            }
        }

        public static void ExplodeGrenade(Player ply)
        {
            try
            {
                Grenade grenade = UnityEngine.Object.Instantiate(ply.GrenadeManager.availableGrenades[0].grenadeInstance).GetComponent<Grenade>();
                grenade.fuseDuration = 0.25f;
                grenade.InitData(ply.ReferenceHub.GetComponent<GrenadeManager>(), Vector3.zero, Vector3.zero, 0);

                Lootorial.Instance.Handler.Grenades.Add(grenade.gameObject);
                NetworkServer.Spawn(grenade.gameObject);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
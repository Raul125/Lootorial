using System.Collections.Generic;
using System.IO;
using DiscordIntegration_Plugin;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Lootorial.Classes;
using MEC;
using NPCS;
using Lootorial.Methods;
using Exiled.API.Extensions;
using System.Linq;
using UnityEngine;
using System;
using Exiled.API.Enums;
using Exiled.Permissions.Extensions;

namespace Lootorial
{
    public class EventHandlers
    {
        private readonly Lootorial plugin;
        public EventHandlers(Lootorial plugin) => this.plugin = plugin;
        public List<GameObject> Pinhatas = new List<GameObject>();
        public List<GameObject> Grenades = new List<GameObject>();
        private static readonly Vector3 AddLaunchForce = new Vector3(0f, 0.25f, 0f);
        private List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        public void OnRoundStarted()
        {
            if (Lootorial.Instance.Config.SpawnOnRoundStart)
            {
                foreach (PinhataPosition pin in Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed)
                {
                    Methods.Methods.CreatePinhata(pin);
                }
            }

            if (Lootorial.Instance.Config.SpawnRandomlyPinhatas > 0)
            {
                Coroutines.Add(Timing.RunCoroutine(Methods.Methods.SpawnRandomlyEveryXseconds()));
            }
        }

        public void OnRestartingRound()
        {
            foreach (CoroutineHandle cor in Coroutines)
            {
                Timing.KillCoroutines(cor);
            }
        }

        public void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            if (ev.Name == "loot" || ev.Name == "lt" || ev.Name == "lootorial")
            {
                if (ev.Sender.CheckPermission("lootorial.main"))
                {
                    ev.IsAllowed = false;

                    if (ev.Arguments.ElementAt(0) == null)
                    {
                        ev.ReplyMessage = "You need to write an argument";
                        ev.Success = true;
                        return;
                    }

                    switch (ev.Arguments.ElementAt(0))
                    {
                        case "point":
                            {
                                if (ev.Sender.CheckPermission("lootorial.point"))
                                {
                                    var scp049Component = ev.Sender.GameObject.GetComponent<Scp049_2PlayerScript>();
                                    var froward = scp049Component.plyCam.transform.forward;
                                    var rotation = new Vector3(-froward.x, froward.y, -froward.z);
                                    var position = ev.Sender.Position + (Vector3.up * 0.1f);
                                    var room = ev.Sender.CurrentRoom;
                                    var p2 = room.Transform.InverseTransformPoint(position);
                                    var r2 = room.Transform.InverseTransformDirection(rotation);
                                    Log.Info($"Room: {ev.Sender.CurrentRoom.Name} Pos:{p2} Rotation:{r2}");
                                    ev.ReplyMessage = $"Room: {ev.Sender.CurrentRoom.Name} Pos:{p2} Rotation:{r2}";
                                    ev.Success = true;
                                    return;
                                }
                                else
                                {
                                    ev.ReplyMessage = "You don't have the requeried permissions";
                                    ev.Success = true;
                                    return;
                                }
                            }
                        case "spawn":
                            {
                                if (ev.Sender.CheckPermission("lootorial.spawn"))
                                {
                                    if (ev.Arguments.ElementAt(1) == null)
                                    {
                                        ev.ReplyMessage = "You need to enter the name";
                                        ev.Success = true;
                                    }

                                    if (RoleType.TryParse<RoleType>(ev.Arguments.ElementAt(2), out var role) == false)
                                    {
                                        ev.ReplyMessage = "You need to enter a valid RoleType";
                                        ev.Success = true;
                                    }
                                    var npc = NPCS.Methods.CreateNPC(ev.Sender.Position, ev.Sender.Rotations, new UnityEngine.Vector3(1, 1, 1), role, ItemType.None, ev.Arguments.ElementAt(1));
                                    Timing.CallDelayed(0.5f, () =>
                                    {
                                        foreach (ItemType it in Lootorial.Instance.Config.DroppableItems)
                                        {
                                            npc.NPCPlayer.AddItem(it);
                                        }
                                    });

                                    return;
                                }
                                else
                                {
                                    ev.ReplyMessage = "You don't have the requeried permissions";
                                    ev.Success = true;
                                    return;
                                }
                            }
                    }
                }
                else
                {
                    ev.IsAllowed = false;
                    ev.ReplyMessage = "You don't have the requeried permissions";
                    ev.Success = true;
                }
            }
        }

        public void OnDying(DyingEventArgs ev)
        {
            if (ev.Target.GameObject == null)
            {
                return;
            }

            if (Pinhatas.Contains(ev.Target.GameObject) == false)
            {
                return;
            }

            try
            {
                foreach (var item in ev.Target.Inventory.items)
                {
                    Pickup pickup = ev.Target.Inventory.SetPickup(item.id, item.durability, ev.Target.Position, ev.Target.Inventory.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
                    Timing.RunCoroutine(Methods.Methods.ThrowItem(pickup, (ev.Target.CameraTransform.forward + AddLaunchForce).normalized));
                }
                if (Lootorial.Instance.Config.Grenade)
                {
                    Methods.Methods.ExplodeGrenade(ev.Target);
                }
            } 
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            if (Grenades.Contains(ev.Grenade))
            {
                if (Lootorial.Instance.Config.GrenadeShouldDamage == false)
                {
                    ev.TargetToDamages.Clear();
                }
                Grenades.Remove(ev.Grenade);
            }
        }

    }
}
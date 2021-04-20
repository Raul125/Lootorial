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
using System.Globalization;

namespace Lootorial
{
    public class EventHandlers
    {
        private readonly Lootorial plugin;
        public EventHandlers(Lootorial plugin) => this.plugin = plugin;
        public List<GameObject> Grenades = new List<GameObject>();
        private static readonly Vector3 AddLaunchForce = new Vector3(0f, 0.25f, 0f);
        private List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
        private List<GameObject> RoundPinhatas = new List<GameObject>();
        private static List<string> FriendlyFireUsers = new List<string>();

        public void OnRoundStarted()
        {
            if (Lootorial.Instance.Config.SpawnOnRoundStart)
            {
                foreach (PinhataPosition pin in Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.Keys)
                {
                    if (Methods.Methods.Rand.Next(100) < Lootorial.Instance.Config.SpawnChance)
                    {
                        Methods.Methods.CreatePinhata(pin);
                    } 
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
            
            Grenades.Clear();
            RoundPinhatas.Clear();
            Coroutines.Clear();
            FriendlyFireUsers.Clear();
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (FriendlyFireUsers.Contains(ev.Attacker.UserId))
            {
                FriendlyFireUsers.Remove(ev.Attacker.UserId);
                ev.Attacker.IsFriendlyFireEnabled = false;
            }

            if (ev.Attacker == ev.Target)
            {
                return;
            }

            if (RoundPinhatas.Contains(ev.Target.GameObject) || Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.ContainsValue(ev.Target.GameObject))
            {
                if (ev.Attacker.Side == ev.Target.Side)
                {
                    ev.Attacker.IsFriendlyFireEnabled = true;
                    FriendlyFireUsers.Add(ev.Attacker.UserId);
                }
            }
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            if (FriendlyFireUsers.Contains(ev.Shooter.UserId))
            {
                FriendlyFireUsers.Remove(ev.Shooter.UserId);
                ev.Shooter.IsFriendlyFireEnabled = false;
            }

            if (ev.Target == null)
            {
                return;
            }

            var target = Player.Get(ev.Target);

            if (RoundPinhatas.Contains(ev.Target) || Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.ContainsValue(ev.Target))
            {
                if (ev.Shooter.Side == target.Side)
                {
                    ev.Shooter.IsFriendlyFireEnabled = true;
                    FriendlyFireUsers.Add(ev.Shooter.UserId);
                }
            }
        }

        public void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.ContainsValue(ev.Owner.GameObject))
            {
                ev.IsAllowed = false;
            }
        }

        // Why im not using command system? I don't think this plugin is going to be used by many people, so I'm not going to complicate my life.
        public void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            if (ev.Name == "lt" || ev.Name == "lootorial")
            {
                ev.IsAllowed = false;
                if (ev.Sender.CheckPermission("lootorial.main"))
                {
                    var arguments = ev.Arguments.ToArray();

                    if (arguments.Count() == 0)
                    {
                        ev.ReplyMessage = "You need to write an argument\npoint\nspawn";
                        ev.Success = true;
                        return;
                    }

                    switch (arguments.ElementAt(0))
                    {
                        case "point":
                            {
                                if (ev.Sender.CheckPermission("lootorial.point"))
                                {
                                    var rotation = ev.Sender.Rotations;
                                    var position = ev.Sender.Position + (Vector3.up * 0.1f);
                                    var room = ev.Sender.CurrentRoom;
                                    var p2 = room.Transform.InverseTransformPoint(position);
                                    var r2 = room.Transform.InverseTransformDirection(rotation);
                                    var lastr = new Vector2(r2.x, r2.y);

                                    Log.Info($"Room: {ev.Sender.CurrentRoom.Name} Pos:{p2} Rotation:{lastr}");
                                    ev.ReplyMessage = $"Room: {ev.Sender.CurrentRoom.Name} Pos:{p2} Rotation:{lastr}";
                                    ev.Success = true;
                                    break;
                                }
                                else
                                {
                                    ev.ReplyMessage = "You don't have the requeried permissions";
                                    ev.Success = true;
                                    break;
                                }
                            }
                        case "spawn":
                            {
                                if (ev.Sender.CheckPermission("lootorial.spawn"))
                                {
                                    if (arguments.Count() == 1)
                                    {
                                        ev.ReplyMessage = "Format: Name RoleType Health\nExample: lt spawn Pinhata Tutorial 100";
                                        ev.Success = true;
                                        return;
                                    }

                                    if (RoleType.TryParse<RoleType>(arguments.ElementAt(2), out var role) == false)
                                    {
                                        ev.ReplyMessage = "You need to enter a valid RoleType";
                                        ev.Success = true;
                                        return;
                                    }

                                    if (float.TryParse(arguments.ElementAt(3), NumberStyles.Float, CultureInfo.InvariantCulture, out float health) == false)
                                    {
                                        ev.ReplyMessage = "You need to enter a valid Health amount";
                                        ev.Success = true;
                                        return;
                                    }

                                    var rotation = ev.Sender.Rotations;

                                    var pin = new PinhataPosition
                                    {
                                        Room = ev.Sender.CurrentRoom.Name,
                                        Role = role,
                                        Name = arguments.ElementAt(1),
                                        Position = ev.Sender.Position,
                                        Direction = rotation,
                                        Scale = new Vector3(1, 1, 1),
                                        Health = health
                                    };

                                    RoundPinhatas.Add(Methods.Methods.SpawnPinhata(pin).gameObject);
                                    ev.ReplyMessage = "Spawned";
                                    ev.Success = true;

                                    break;
                                }
                                else
                                {
                                    ev.ReplyMessage = "You don't have the requeried permissions";
                                    ev.Success = true;
                                    break;
                                }
                            }
                    }
                }
                else
                {
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

            if (Lootorial.Instance.Config.PinhatasPositions.PinhatasParsed.ContainsValue(ev.Target.GameObject) == true || RoundPinhatas.Contains(ev.Target.GameObject) == true)
            {
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

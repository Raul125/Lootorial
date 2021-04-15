using System;
using Exiled.API.Features;
using Exiled.API.Enums;
using ServerEv = Exiled.Events.Handlers.Server;
using PlayerEv = Exiled.Events.Handlers.Player;
using MapEv = Exiled.Events.Handlers.Map;
using System.Linq;

namespace Lootorial
{
    public class Lootorial : Plugin<PluginConfig>
    {
        internal static Lootorial Instance { get; } = new Lootorial();
        private Lootorial()
        {

        }

        private NPCS.Plugin NpcPlugin;

        public override PluginPriority Priority { get; } = PluginPriority.Lowest;

        public override string Author { get; } = "Raul125";
        public override string Name { get; } = "Lootorial";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 8);

        public EventHandlers Handler;
        public static Lootorial Singleton;


        public override void OnEnabled()
        {
            try
            {
                base.OnEnabled();

                NpcPlugin = (NPCS.Plugin)Exiled.Loader.Loader.Plugins.FirstOrDefault(x => x.Name == "CustomNPCs");

                if (NpcPlugin == null)
                {
                    Log.Error("Lootorial won't work, Npcs plugin isn't in the plugins directory");
                    return;
                }

                if (NpcPlugin.Config.IsEnabled == false)
                {
                    Log.Error("Lootorial won't work, Npc plugin IsEnabled == false");
                    return;
                }

                RegisterEvents();
            }
            catch (Exception e)
            {
                Log.Error($"There was an error loading the plugin: {e}");
            }
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            UnregisterEvents();
        }

        public void RegisterEvents()
        {
            Singleton = this;
            Handler = new EventHandlers(this);
            Config.PinhatasPositions.ParsePositions();
            ServerEv.RoundStarted += Handler.OnRoundStarted;
            ServerEv.RestartingRound += Handler.OnRestartingRound;
            ServerEv.SendingRemoteAdminCommand += Handler.OnSendingRemoteAdminCommand;
            PlayerEv.Dying += Handler.OnDying;
            MapEv.ExplodingGrenade += Handler.OnExploding;
        }
        public void UnregisterEvents()
        {
            ServerEv.RoundStarted -= Handler.OnRoundStarted;
            ServerEv.RestartingRound -= Handler.OnRestartingRound;
            ServerEv.SendingRemoteAdminCommand -= Handler.OnSendingRemoteAdminCommand;
            PlayerEv.Dying -= Handler.OnDying;
            MapEv.ExplodingGrenade -= Handler.OnExploding;
            Handler = null;
        }

        public void OnReload()
        {
            Config.PinhatasPositions.PinhatasParsed.Clear();
            Config.PinhatasPositions.ParsePositions();
        }
    }
}

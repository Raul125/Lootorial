using Exiled.API.Features;
using Exiled.API.Interfaces;
using Lootorial.Classes;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Lootorial
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("Should spawn Pinhatas on round start?")]
        public bool SpawnOnRoundStart { get; set; } = true;

        [Description("Every x seconds it will try to spawn a pinhata, 0 is disabled")]
        public float SpawnRandomlyPinhatas { get; set; } = 200;

        [Description("Chance of spawn for the pinhatas")]
        public int SpawnChance { get; set; } = 40;

        [Description("Displayed broadcast when a randomly pinhata spawns")]
        public string PinhataBroadcast { get; set; } = "<color=cyan>A</color><color=green><b> Pinhata </b></color><color=cyan>has spawned in<b> %room </b>Room</color>";

        [Description("Max Items per pinhata")]
        public int MaxItemsPerPinhata { get; set; } = 8;

        [Description("Should a grenade explode?")]
        public bool Grenade { get; set; } = true;

        [Description("Grenade should damage?")]
        public bool GrenadeShouldDamage { get; set; } = false;

        [Description("Throw force of the items, it can be negative")]
        public float ThrowForce { get; set; } = 17f;

        [Description("setting to 0 disables the random spin, otherwise the items will randomly spin")]
        public float RandomSpinForce { get; set; } = 20f;

        [Description("List of the Possible items of the Pinhatas")]
        public List<ItemType> DroppableItems { get; set; } = new List<ItemType>()
        {
            { ItemType.Flashlight },
            { ItemType.Radio },
            { ItemType.MicroHID }
		};

        public PinhatasPositionsConfig PinhatasPositions { get; set; } = new PinhatasPositionsConfig();
    }
}
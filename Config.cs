using Exiled.API.Features;
using Exiled.API.Interfaces;
using Lootorial.Classes;
using System.Collections.Generic;
using System.IO;

namespace Lootorial
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool SpawnOnRoundStart { get; set; } = true;

        public float SpawnRandomlyPinhatas { get; set; } = 90;

        public bool Grenade { get; set; } = true;

        public bool GrenadeShouldDamage { get; set; } = false;

        public List<ItemType> DroppableItems { get; set; } = new List<ItemType>()
        {
            { ItemType.Flashlight },
            { ItemType.Radio },
            { ItemType.MicroHID }
		};

        public PinhatasPositionsConfig PinhatasPositions { get; set; } = new PinhatasPositionsConfig();
    }
}
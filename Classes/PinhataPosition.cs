using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Linq;
using UnityEngine;

namespace Lootorial.Classes
{
    [Serializable]
    public class PinhataPosition
    {
        public string Room;

        public RoleType Role;

        public Vector3 Position;

        public Vector3 Direction;

        public string Name;

        public override string ToString() => $"{Room}:{Position.x}:{Position.y}:{Position.z}";
    }
}
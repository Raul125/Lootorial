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

        public Vector3 Scale;

        public Vector2 Direction;

        public float Health;

        public string Name;
    }
}
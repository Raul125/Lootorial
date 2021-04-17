using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Exiled.API.Enums;
using Exiled.API.Features;
using Lootorial.Classes;

namespace Lootorial.Classes
{
	[Serializable]
	public class PinhatasPositionsConfig
    {
        [Description("Name:RoleType:RoomName:Position:Rotation:Scale:Health")]
		public List<string> Positions { set; get; } = new List<string>
		{
            { "Pinhata:ClassD:LCZ_ClassDSpawn (1):-31.1,1.4,-0.2:0.5,90.1:0.9,0.9,0.9:100" }
		};

        internal Dictionary<PinhataPosition, UnityEngine.GameObject> PinhatasParsed { set; get; } = new Dictionary<PinhataPosition, UnityEngine.GameObject>();

        internal void ParsePositions()
        {
            foreach (string st in Positions)
            {
                try
                {
                    var splited = st.Split(':');
                    var room = splited[2];
                    var name = splited[0];
                    
                    if (name == null)
                    {
                        Log.Error($"Error while parsing pinhatas config: A Name is empty");
                        name = "Pinhata";
                    }

                    if (room == null)
                    {
                        Log.Error($"Error while parsing pinhatas config: A RoomName is empty");
                        break;
                    }

                    if (RoleType.TryParse<RoleType>(splited[1], out var role) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: {splited[1]} RoleType isn't valid");
                        role = RoleType.Tutorial;
                    }

                    var Pos = splited[3].Split(',');
                    var Dir = splited[4].Split(',');
                    var Sc = splited[5].Split(',');

                    if (float.TryParse(Pos[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: Position {splited[3]} isn't a valid Vector3");
                        break;
                    }
                    if (float.TryParse(Pos[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: Position {splited[3]} isn't a valid Vector3");
                        break;
                    }
                    if (float.TryParse(Pos[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: Position {splited[3]} isn't a valid Vector3");
                        break;
                    }

                    if (float.TryParse(Dir[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float xR) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: Direction {splited[4]} isn't a valid Vector3");
                        break;
                    }
                    if (float.TryParse(Dir[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float yR) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: Direction {splited[4]} isn't a valid Vector3");
                        break;
                    }

                    if (float.TryParse(Sc[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float xS) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: Scale {splited[5]} isn't a valid Vector3");
                        xS = 1;
                    }
                    if (float.TryParse(Sc[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float yS) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: Scale {splited[5]} isn't a valid Vector3");
                        yS = 1;
                    }
                    if (float.TryParse(Sc[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float zS) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: Scale {splited[5]} isn't a valid Vector3");
                        zS = 1;
                    }

                    if (float.TryParse(splited[6], NumberStyles.Float, CultureInfo.InvariantCulture, out float health) == false)
                    {
                        Log.Error($"Error while parsing pinhatas config: This {splited[6]} isn't a valid float");
                        health = 100;
                    }

                    PinhatasParsed.Add(
                    new PinhataPosition
                    {
                        Room = room,
                        Role = role,
                        Name = name,
                        Position = new UnityEngine.Vector3(x, y, z),
                        Direction = new UnityEngine.Vector2(xR, yR),
                        Scale = new UnityEngine.Vector3(xS, yS, zS),
                        Health = health
                    }, null);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to parse the pinhatas list: {e}");
                }
            }
        }
    }
}
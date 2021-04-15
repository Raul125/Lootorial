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
        [Description("The stationary options list.")]
		public List<string> Positions { set; get; } = new List<string>
		{
            { "Test:Tutorial:HCZ_Servers:-1.0,-6.5,8.4:0.9,-0.1,-0.3" }
		};

        internal List<PinhataPosition> PinhatasParsed { set; get; } = new List<PinhataPosition>();

        internal void ParsePositions()
        {
            foreach (string st in Positions)
            {
                try
                {
                    var splited = st.Split(':');

                    var room = splited[2];
                    RoleType.TryParse<RoleType>(splited[1], out var role);

                    var Pos = splited[3].Split(',');
                    var Dir = splited[4].Split(',');

                    var x = float.Parse(Pos[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var y = float.Parse(Pos[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var z = float.Parse(Pos[2], NumberStyles.Float, CultureInfo.InvariantCulture);

                    var xR = float.Parse(Dir[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var yR = float.Parse(Dir[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var zR = float.Parse(Dir[2], NumberStyles.Float, CultureInfo.InvariantCulture);

                    var pose = new UnityEngine.Vector3(x, y, z);

                    PinhatasParsed.Add(
                    new PinhataPosition
                    {
                        Room = room,
                        Role = role,
                        Name = splited[0],
                        Position = pose,
                        Direction = new UnityEngine.Vector3(xR, yR, zR)
                    });
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to parse the positions list {e}");
                }
            }
        }
    }
}
// Author: Grant Nations
// Author: Sebastian Ramirez
// Control command class for CS 3500 TankWars Client (PS8)

using System;
using System.Collections.Generic;
using System.Text;
using TankWars;
using Newtonsoft.Json;

namespace GameModel
{
    [JsonObject]
    public class ControlCmd
    {
        [JsonProperty]
        public string moving { get; set; }

        [JsonProperty]
        public string fire { get; set; }

        [JsonProperty]
        public Vector2D tdir { get; set; }

        public ControlCmd() { }

    }
}

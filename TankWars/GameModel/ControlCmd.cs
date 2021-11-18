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
        // TODO: Normalize vector
        [JsonProperty]
        public string moving { get; private set; }

        [JsonProperty]
        public string fire { get; private set; }

        [JsonProperty]
        public Vector2D tdir { get; private set; }

        public ControlCmd() { }
    }
}

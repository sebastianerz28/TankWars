using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace GameModel
{
    [JsonObject]
    public class Powerup
    {
        [JsonProperty(PropertyName = "power")]
        public int id { get; private set; }

        [JsonProperty]
        public Vector2D loc { get; private set; }

        [JsonProperty]
        public bool died { get; private set; }

        public Powerup() { }
    }
}

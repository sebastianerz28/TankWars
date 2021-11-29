// Author: Grant Nations
// Author: Sebastian Ramirez
// Powerup class for CS 3500 TankWars Client (PS8)

using Newtonsoft.Json;
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
    }
}

// Author: Grant Nations
// Author: Sebastian Ramirez
// Wall class for CS 3500 TankWars Client (PS8)

using Newtonsoft.Json;
using TankWars;

namespace GameModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        [JsonProperty(PropertyName = "wall")]
        public int id { get; private set; }

        [JsonProperty]
        public Vector2D p1 { get; private set; }

        [JsonProperty]
        public Vector2D p2 { get; private set; }
    }
}

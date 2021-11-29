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
        public int ID { get; private set; }

        [JsonProperty(PropertyName = "p1")]
        public Vector2D P1 { get; private set; }

        [JsonProperty(PropertyName = "p2")]
        public Vector2D P2 { get; private set; }
    }
}

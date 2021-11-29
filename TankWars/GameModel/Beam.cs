// Author: Grant Nations
// Author: Sebastian Ramirez
// Beam class for CS 3500 TankWars Client (PS8)

using TankWars;
using Newtonsoft.Json;
namespace GameModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        public int id { get; private set; }

        [JsonProperty(PropertyName = "org")]
        public Vector2D org { get; private set; }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D dir { get; private set; }

        [JsonProperty(PropertyName = "owner")]
        public int owner { get; private set; }
    }
}

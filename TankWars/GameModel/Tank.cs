// Author: Grant Nations
// Author: Sebastian Ramirez
// Tank class for CS 3500 TankWars Client (PS8)

using Newtonsoft.Json;
using TankWars;

namespace GameModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {

        [JsonProperty(PropertyName = "tank")]
        public int ID { get; private set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location { get; private set; }

        [JsonProperty(PropertyName = "bdir")]
        public Vector2D orientation { get; private set; }

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D aiming { get; private set; }

        [JsonProperty]
        public string name { get; private set; }

        [JsonProperty]
        public int hp { get; private set; }

        [JsonProperty]
        public int score { get; private set; }

        [JsonProperty]
        public bool died { get; private set; }

        [JsonProperty(PropertyName = "dc")]
        public bool disconnected { get; private set; }

        [JsonProperty(PropertyName = "join")]
        public bool joined { get; private set; }

        /// <summary>
        /// Default constructor for JSON serialization
        /// </summary>
        public Tank()
        {
            aiming = new Vector2D(0,-1);
            score = 0;
            died = false;
            disconnected = false;
            joined = false;
        }
    }  
}

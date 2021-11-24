using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
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

        [JsonProperty(PropertyName = "name")]
        public string name { get; private set; }

        [JsonProperty(PropertyName = "hp")]
        public int hp { get; private set; }

        [JsonProperty(PropertyName = "score")]
        public int score { get; private set; }

        [JsonProperty(PropertyName = "died")]
        public bool died { get; private set; }

        [JsonProperty(PropertyName = "dc")]
        public bool disconnected { get; private set; }

        [JsonProperty(PropertyName = "join")]
        public bool joined { get; private set; }

        

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

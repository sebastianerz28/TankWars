﻿// Author: Grant Nations
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
        public Vector2D Location { get; private set; }

        [JsonProperty(PropertyName = "bdir")]
        public Vector2D Orientation { get; private set; }

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D Aiming { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        [JsonProperty(PropertyName = "hp")]
        public int HP { get; private set; }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; private set; }

        [JsonProperty(PropertyName = "died")]
        public bool Died { get; private set; }

        [JsonProperty(PropertyName = "dc")]
        public bool Disconnected { get; private set; }

        [JsonProperty(PropertyName = "join")]
        public bool Joined { get; private set; }

        /// <summary>
        /// Default Tank constructor
        /// </summary>
        public Tank()
        {
            Aiming = new Vector2D(0,-1);
            Score = 0;
            Died = false;
            Disconnected = false;
            Joined = false;
        }
    }  
}

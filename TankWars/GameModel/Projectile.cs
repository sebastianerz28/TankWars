// Author: Grant Nations
// Author: Sebastian Ramirez
// Projectile class for CS 3500 TankWars Client (PS8)

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace GameModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        public int ID { get; private set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; private set; }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D Direction { get; private set; }

        [JsonProperty(PropertyName = "died")]
        public bool Died { get; private set; }

        [JsonProperty(PropertyName = "owner")]
        public int Owner { get; private set; }
    }
}

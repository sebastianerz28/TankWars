using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace GameModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        [JsonProperty(PropertyName = "wall")]
        public int id { get; private set; }

        [JsonProperty(PropertyName = "p1")]
        public Vector2D p1 { get; private set; }

        [JsonProperty(PropertyName = "p2")]
        public Vector2D p2 { get; private set; }

        public Wall()
        {

        }



    }
}

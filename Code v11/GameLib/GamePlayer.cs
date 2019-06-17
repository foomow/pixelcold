using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GameLib
{
    class GamePlayer : GameCharacter
    {
        public uint AID = 0;
        public uint PlayerID = 10000000;
        public DateTime CreateTime = DateTime.Now;
        public string[] NameList = new string[4];

        public GamePlayer(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.PLAYER;
        }

        public override JObject ToJson()
        {
            JObject ClientJSON = base.ToJson();
            ClientJSON.Add("AID", AID);
            ClientJSON.Add("PlayerID", PlayerID);
            ClientJSON.Add("NameList", JArray.FromObject(NameList));
            return ClientJSON;
        }
    }
}

using System;
using Newtonsoft.Json.Linq;

namespace GameLib
{
    class GameCharacter : GameObject
    {
        public byte Gender = 0;//0-female, 1-male
        public uint Level = 1;
        public uint Exp = 0;
        public byte HP = 100;
        public byte Max_HP = 100;
        public byte Energe = 100;
        public byte Max_Energe = 100;
        public byte Hungry = 100;
        public byte Max_Hungry = 100;
        public ushort Attack_Point = 1;
        public ushort Defence_Point = 1;
        public ushort Attack_Point_Extra = 1;
        public ushort Defence_Point_Extra = 1;

        public byte Speed = 128;
        private byte SpeedBuff = 0;

        public event CrossMapHandler CrossMap;

        public override JObject ToJson()
        {
            JObject ClientJSON = base.ToJson();
            ClientJSON.Add("Gender", Gender);
            ClientJSON.Add("Level", Level);
            ClientJSON.Add("Exp", Exp);
            ClientJSON.Add("HP", HP);
            ClientJSON.Add("Max_HP", Max_HP);
            ClientJSON.Add("Energe", Energe);
            ClientJSON.Add("Max_Energe", Max_Energe);
            ClientJSON.Add("Hungry", Hungry);
            ClientJSON.Add("Max_Hungry", Max_Hungry);
            ClientJSON.Add("Attack_Point", Attack_Point);
            ClientJSON.Add("Defence_Point", Defence_Point);
            ClientJSON.Add("Attack_Point_Extra", Attack_Point_Extra);
            ClientJSON.Add("Defence_Point_Extra", Defence_Point_Extra);
            ClientJSON.Add("Speed", Speed);
            return ClientJSON;
        }

        public bool IsHeartReady
        {
            get
            {
                if (Speed + SpeedBuff == 255)
                    return true;
                else
                    return false;

            }
        }

        public GameCharacter(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.CHARACTER;
        }

        public override void HeartBeat()
        {
            if (Speed + SpeedBuff == 255)
            {
                SpeedBuff = 0;
                base.HeartBeat();
            }
            SpeedBuff++;
        }

        public bool Move(Direction dir)
        {
            bool ret = false;
            switch (dir)
            {
                case Direction.NONE:
                    ret = true;
                    break;
                case Direction.EAST:
                    if (X < (G.MapSize - 1))
                    {
                        X++;
                        ret = true;
                    }
                    else
                    {
                        CrossMap?.Invoke(this, dir);
                    }
                    break;
                case Direction.WEST:
                    if (X > 0)
                    {
                        X--;
                        ret = true;
                    }
                    else
                    {
                        CrossMap?.Invoke(this, dir);
                    }
                    break;
                case Direction.NORTH:
                    if (Y > 0)
                    {
                        Y--;
                        ret = true;
                    }
                    else
                    {
                        CrossMap?.Invoke(this, dir);
                    }
                    break;
                case Direction.SOUTH:
                    if (Y < (G.MapSize - 1))
                    {
                        Y++;
                        ret = true;
                    }
                    else
                    {
                        CrossMap?.Invoke(this, dir);
                    }
                    break;
                default:
                    break;
            }

            return ret;
        }

    }


    class GameNPC : GameCharacter
    {
        public GameNPC(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.NPC;
        }
    }
}

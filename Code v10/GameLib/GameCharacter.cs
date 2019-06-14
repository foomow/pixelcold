using System;

namespace GameLib
{
    class GameCharacter : GameObject
    {
        public byte Speed = 128;
        private byte SpeedBuff = 0;


        public event CrossMapHandler CrossMap;

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
                        CrossMap?.Invoke(this,dir);
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
                        CrossMap?.Invoke(this,dir);
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
                        CrossMap?.Invoke(this,dir);
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
                        CrossMap?.Invoke(this,dir);
                    }
                    break;
                default:
                    break;
            }

            return ret;
        }

    }

    class GamePlayer : GameCharacter
    {
        public GamePlayer(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.PLAYER;
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

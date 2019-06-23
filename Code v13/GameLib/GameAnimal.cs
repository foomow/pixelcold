using System;

namespace GameLib
{
    class GameAnimal : GameCharacter
    {
        public GameAnimal(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.ANIMAL;
        }

        public override void HeartBeat()
        {
            base.HeartBeat();
            if (IsHeartReady)
            {
                Direction dir = (Direction)G.RND((int)Direction.EAST, (int)Direction.SOUTH);
                //LOG(Name + " 向 " + dir.ToString() + " 走去。");
                if (Move(dir))
                {
                    //LOG("当前坐标(" + X + ":" + Y + ")");
                }
                else
                {
                    //LOG("到达地图边界");
                }
            }
        }
    }
}

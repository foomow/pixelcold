using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class GameObject : Existence
    {
        public string Description;
        public ExistenceID MapEID;
        public byte X;
        public byte Y;
        public ObjectCategory Category;
        public ObjectClassID ClassID;
        public ObjectStatus Status;
        public byte StatusStep;
        public byte ImgFileID;
        public byte ImgID;
        public bool CanPass;
        public GameObject[] Inventory;
        public ExistenceID OwnerEID;



        public GameObject(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            LOG("GameObject(" + EID + ")创建成功");
        }

        public override void HeartBeat()
        {
            //LOG("GameObject[" + Name + "](" + EID + ")正在心跳 " + G.GlobeTime);
        }
    }

    class GameTerrain : GameObject
    {
        public GameTerrain(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.TERRAIN;
        }
    }
    
    class GameCarriable : GameObject
    {
        public GameCarriable(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.CARRIABLE;
        }
    }
    class GameMaterial : GameCarriable
    {
        public GameMaterial(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.MATERIAL;
        }
    }
    class GameEquip : GameCarriable
    {
        public GameEquip(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.EQUIP;
        }
    }
    class GameWeapon : GameEquip
    {
        public GameWeapon(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.WEAPON;
        }
    }
    class GameArmor : GameEquip
    {
        public GameArmor(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.ARMOR;
        }
    }
    class GameHerb : GameCarriable
    {
        public GameHerb(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.HERB;
        }
    }
    class GameCurrency : GameCarriable
    {
        public GameCurrency(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.CURRENCY;
        }
    }
    class GameResource : GameObject
    {
        public GameResource(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.RESOURCE;
        }
    }
    class GamePlant : GameResource
    {
        public GamePlant(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.PLANT;
        }
    }
    class GameRock : GameResource
    {
        public GameRock(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            Category = ObjectCategory.ROCK;
        }
    }
}

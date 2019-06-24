using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class ObjectFactory
    {
        public static GameObject Make(ObjectClassID ClassID, Action<string> LOGFUN = null)
        {
            GameObject ret = null;
            switch (ClassID)
            {
                case ObjectClassID.NONE:
                    break;
                case ObjectClassID.DOG:
                    {
                        ret = new GameAnimal(LOGFUN)
                        {
                            Name = "小狗旺财",
                            Description = "一只小狗，就是你",
                            CanPass = false,
                            ClassID = ClassID,
                            ImgFileID = 0,
                            ImgID = 0,
                            Status = ObjectStatus.IDEL,
                            StatusStep = 0,
                            OwnerEID = null,
                            Inventory = new GameObject[8]//可以带8个物品
                        };
                    }
                    break;
                case ObjectClassID.CAT:
                    {
                        ret = new GameAnimal(LOGFUN)
                        {
                            Name = "小猫咪咪",
                            Speed = 60,
                            Description = "一只小猫，好可爱",
                            CanPass = false,
                            ClassID = ClassID,
                            ImgFileID = 0,
                            ImgID = 2,
                            Status = ObjectStatus.IDEL,
                            StatusStep = 0,
                            OwnerEID = null,
                            Inventory = new GameObject[8]//可以带8个物品
                        };
                    }
                    break;
                case ObjectClassID.GRASS:
                    break;
                case ObjectClassID.ROCK:
                    break;
                default:
                    break;
            }
            return ret;
        }
    }
}

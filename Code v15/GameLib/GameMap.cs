using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    class GameMap : Existence
    {

        public readonly List<GameObject> gObjects;

        public ExistenceID NorthMap = ExistenceID.Null;
        public ExistenceID SouthMap = ExistenceID.Null;
        public ExistenceID WestMap = ExistenceID.Null;
        public ExistenceID EastMap = ExistenceID.Null;
        public DateTime LastRefreshTime;
        public List<MapItem> MapItems = new List<MapItem>();
        public CrossMapHandler ObjectCrossMapMethod;

        public GameMap(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            gObjects = new List<GameObject>();
            LOG("GameMap(" + EID + ")创建成功");
        }

        public override void HeartBeat()
        {
            //LOG("GameMap(" + Name + ")正在心跳 " + G.GlobeTime);
            GameObject[] ObjectList;
            lock (gObjects)
            {
                //ObjectList = new GameObject[gObjects.Count];
                ObjectList = gObjects.ToArray();
            }
            foreach (GameObject OBJ in ObjectList)
            {
                if (OBJ != null)
                    OBJ.HeartBeat();
            }
        }

        public void Refresh()//刷新
        {
            LOG("重新生成地图(" + EID + ")内容");
            lock (this)
            {
                gObjects.Clear();
                foreach (MapItem item in MapItems)
                {
                    GameObject obj = ObjectFactory.Make(item.ClassID, LOG);
                    if (obj != null)
                    {
                        obj.MapEID = EID;
                        obj.X = item.X;
                        obj.Y = item.Y;
                        if (obj.Category == ObjectCategory.ANIMAL || obj.Category == ObjectCategory.CHARACTER)
                        {
                            ((GameCharacter)obj).CrossMap += new CrossMapHandler(ObjectCrossMapMethod);
                        }
                        gObjects.Add(obj);

                        LOG("加入 " + obj.Name + " 成功");
                    }
                }
            }
            LastRefreshTime = DateTime.Now;
        }

        internal void AddObject(GameObject obj)
        {
            obj.MapEID = EID;
            lock (gObjects)
            {
                gObjects.Add(obj);
            }
        }

        internal void RemoveObject(GameObject obj)
        {
            lock (gObjects)
            {
                gObjects.Remove(obj);
            }
        }

        internal GameObject[] GetObjsAtXY(byte x, byte y)
        {
            List<GameObject> ret = new List<GameObject>();
            GameObject[] ObjectList;
            lock (gObjects)
            {
                ObjectList = gObjects.ToArray();
            }
            foreach (GameObject OBJ in ObjectList)
            {
                if (OBJ != null && OBJ.X == x && OBJ.Y == y)
                    ret.Add(OBJ);
            }
            return ret.ToArray();
        }
    }
}

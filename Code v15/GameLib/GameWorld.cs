using System;
using System.Collections.Generic;

namespace GameLib
{
    class GameWorld : Existence
    {
        private readonly List<GameMap> gMaps;
        public GameWorld(Action<string> LOGFUN = null) : base(LOGFUN)
        {
            gMaps = new List<GameMap>();
            LOG("GameWorld(" + EID + ")创建成功");
            GameMap Map = new GameMap(LOG);
            Map.Name = "中央地图";
            Map.MapItems.Add(new MapItem()
            {
                ClassID = ObjectClassID.DOG,
                X = 1,
                Y = 1
            }
            );
            Map.MapItems.Add(new MapItem()
            {
                ClassID = ObjectClassID.CAT,
                X = 1,
                Y = 2
            }
            );
            Map.MapItems.Add(new MapItem()
            {
                ClassID = ObjectClassID.DOG,
                X = 1,
                Y = 1
            }
            );
            Map.MapItems.Add(new MapItem()
            {
                ClassID = ObjectClassID.CAT,
                X = 1,
                Y = 2
            }
            );
            Map.MapItems.Add(new MapItem()
            {
                ClassID = ObjectClassID.DOG,
                X = 1,
                Y = 1
            }
            );
            Map.MapItems.Add(new MapItem()
            {
                ClassID = ObjectClassID.CAT,
                X = 1,
                Y = 2
            }
            );
            Map.MapItems.Add(new MapItem()
            {
                ClassID = ObjectClassID.DOG,
                X = 1,
                Y = 1
            }
            );
            Map.MapItems.Add(new MapItem()
            {
                ClassID = ObjectClassID.CAT,
                X = 1,
                Y = 2
            }
            );
            Map.ObjectCrossMapMethod = ObjectCrossToMap;
            Map.Refresh();
            gMaps.Add(Map);

            GameMap MapEast = new GameMap(LOG);
            MapEast.Name = "东部地图";
            MapEast.WestMap = Map.EID;
            Map.EastMap = MapEast.EID;
            Map.ObjectCrossMapMethod = ObjectCrossToMap;
            MapEast.Refresh();
            gMaps.Add(MapEast);

            GameMap MapSouth = new GameMap(LOG);
            MapSouth.Name = "南部地图";
            MapSouth.NorthMap = Map.EID;
            Map.SouthMap = MapSouth.EID;
            Map.ObjectCrossMapMethod = ObjectCrossToMap;
            MapSouth.Refresh();
            gMaps.Add(MapSouth);
        }

        public override void HeartBeat()
        {
            //LOG("GameWorld(" + EID + ")正在心跳 " + G.GlobeTime);
            GameMap[] MapList;
            lock (gMaps)
            {
                MapList = gMaps.ToArray();
            }
            foreach (GameMap Map in MapList)
            {
                if (Map != null)
                    Map.HeartBeat();
            }
        }

        private void ObjectCrossToMap(GameObject OBJ, Direction dir)
        {
            GameMap Map = GetMap(OBJ.MapEID);
            //LOG(OBJ.Name + " 尝试从 "+Map.Name+" 向 "+dir + " 越地图边界");
            ExistenceID NewMapID = ExistenceID.Null;

            if (Map != null)
            {
                switch (dir)
                {
                    case Direction.EAST:
                        NewMapID = Map.EastMap;
                        break;
                    case Direction.SOUTH:
                        NewMapID = Map.SouthMap;
                        break;
                    case Direction.WEST:
                        NewMapID = Map.WestMap;
                        break;
                    case Direction.NORTH:
                        NewMapID = Map.NorthMap;
                        break;
                }
                GameMap NewMap = GetMap(NewMapID);
                if (NewMap != null)
                {
                    Map.gObjects.Remove(OBJ);
                    switch (dir)
                    {
                        case Direction.EAST:
                            OBJ.X = 0;
                            break;
                        case Direction.SOUTH:
                            OBJ.Y = 0;
                            break;
                        case Direction.WEST:
                            OBJ.X = (byte)(G.MapSize - 1);
                            break;
                        case Direction.NORTH:
                            OBJ.Y = (byte)(G.MapSize - 1);
                            break;
                    }
                    OBJ.MapEID = NewMap.EID;
                    NewMap.gObjects.Add(OBJ);
                    //LOG(OBJ.Name + " 越地图边界到达 "+NewMap.Name+" (X:" + OBJ.X + "，Y:" + OBJ.Y + ")");
                }
                else
                {
                    //LOG(OBJ.Name + " 越地图边界失败，前方没有地图。");
                }
                OBJ.Status = ObjectStatus.IDEL;
            }
        }

        private GameMap GetMap(ExistenceID mapEID)
        {
            return gMaps.Find(x => x.EID.Equals(mapEID));
        }

        internal bool AddObjectIntoMap(GameObject Obj, ExistenceID MapEID)
        {
            GameMap map = GetMap(MapEID);
            if (map == null)
            {
                map = gMaps[0];
            }

            map.AddObject(Obj);
            return true;
        }

        internal void RemoveObjectFromMap(GameObject Obj, ExistenceID MapEID)
        {
            GameMap map = GetMap(MapEID);
            if (map != null)
            {
                map.RemoveObject(Obj);
                Obj.LOG(Obj.Name + " 离开了……");
            }
        }

        internal void MakeRenderInfos(GameObject Obj)
        {
            lock (Obj.RenderInfos)
            {
                Obj.RenderInfos.Clear();
                GameMap map = GetMap(Obj.MapEID);
                if (map != null)
                {
                    for (byte vx = 0; vx < 13; vx++)
                        for (byte vy = 0; vy < 13; vy++)
                        {
                            GameObject[] Objs = GetObjs(map, Obj.X - 6 + vx, Obj.Y - 6 + vy);
                            if (Objs != null)
                            {
                                foreach (GameObject obj in Objs)
                                {
                                    RenderInfo info = obj.RenderInfo;
                                    info.X = vx;
                                    info.Y = vy;
                                    Obj.RenderInfos.Add(info);
                                }
                            }
                            else
                            {
                                RenderInfo info = new RenderInfo()
                                {
                                    ImgID = 9
                                };
                                info.X = vx;
                                info.Y = vy;
                                Obj.RenderInfos.Add(info);
                            }
                        }
                }
            }
        }

        internal GameObject[] GetObjs(GameMap map, int x, int y)
        {
            GameMap ActualMap = map;
            if ((x > G.MapSize - 1) && (y > G.MapSize - 1))
            {
                ActualMap = GetMap(map.EastMap);
                if (ActualMap == null)
                {
                    ActualMap = GetMap(map.SouthMap);
                    if (ActualMap == null) return null;
                    ActualMap = GetMap(ActualMap.EastMap);
                    if (ActualMap == null) return null;
                }
                else
                {
                    ActualMap = GetMap(map.SouthMap);
                    if (ActualMap == null) return null;
                }

                x = x - G.MapSize;
                y = y - G.MapSize;
            }

            if ((x > G.MapSize - 1) && (y < 0))
            {
                ActualMap = GetMap(map.EastMap);
                if (ActualMap == null)
                {
                    ActualMap = GetMap(map.NorthMap);
                    if (ActualMap == null) return null;
                    ActualMap = GetMap(ActualMap.EastMap);
                    if (ActualMap == null) return null;
                }
                else
                {
                    ActualMap = GetMap(map.NorthMap);
                    if (ActualMap == null) return null;
                }
                x = x - G.MapSize;
                y = y + G.MapSize;
            }

            if ((x < 0) && (y > G.MapSize - 1))
            {
                ActualMap = GetMap(map.WestMap);
                if (ActualMap == null)
                {
                    ActualMap = GetMap(map.SouthMap);
                    if (ActualMap == null) return null;
                    ActualMap = GetMap(ActualMap.WestMap);
                    if (ActualMap == null) return null;
                }
                else
                {
                    ActualMap = GetMap(map.SouthMap);
                    if (ActualMap == null) return null;
                }
                x = x + G.MapSize;
                y = y - G.MapSize;
            }

            if ((x < 0) && (y < 0))
            {
                ActualMap = GetMap(map.WestMap);
                if (ActualMap == null)
                {
                    ActualMap = GetMap(map.NorthMap);
                    if (ActualMap == null) return null;
                    ActualMap = GetMap(ActualMap.WestMap);
                    if (ActualMap == null) return null;
                }
                else
                {
                    ActualMap = GetMap(map.NorthMap);
                    if (ActualMap == null) return null;
                }
                x = x + G.MapSize;
                y = y + G.MapSize;
            }

            if (x > G.MapSize - 1)
            {
                ActualMap = GetMap(map.EastMap);
                if (ActualMap == null) return null;
                x = x - G.MapSize;
            }
            if (x < 0)
            {
                ActualMap = GetMap(map.WestMap);
                if (ActualMap == null) return null;
                x = x + G.MapSize;
            }
            if (y > G.MapSize - 1)
            {
                ActualMap = GetMap(map.SouthMap);
                if (ActualMap == null) return null;
                y = y - G.MapSize;
            }
            if (y < 0)
            {
                ActualMap = GetMap(map.NorthMap);
                if (ActualMap == null) return null;
                y = y + G.MapSize;
            }
            return ActualMap.GetObjsAtXY((byte)x, (byte)y);
        }
    }
}

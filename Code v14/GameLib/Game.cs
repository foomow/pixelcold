using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameLib
{
    public class Game
    {
        private readonly GameWorld gWorld;
        private Thread RunThread;
        private bool IsRunning;
        private Action<string> LOG;
        public string Name;
        private List<GamePlayer> PlayerPool;
        public Game(Action<string> LOGFUN = null)
        {
            G.GlobeTime = 0;
            gWorld = new GameWorld(LOGFUN);
            PlayerPool = new List<GamePlayer>();
            RunThread = null;
            IsRunning = false;
            LOG = LOGFUN;
            LOG("Game类创建成功");
        }

        public void Run()
        {

            IsRunning = true;
            if (RunThread == null)
            {
                RunThread = new Thread(() =>
                {
                    while (IsRunning)
                    {
                        //LOG("Game 正在运行");
                        if (G.GlobeTime % 1000 == 0)
                        {
                            gWorld.HeartBeat();
                        }
                        _ = G.GlobeTime == uint.MaxValue ? G.GlobeTime = 0 : G.GlobeTime++;

                    }
                    LOG("Game 即将停止");
                });
            }
            RunThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            while (RunThread != null && RunThread.ThreadState != ThreadState.Stopped)
            {
                LOG("Game 正在停止，请稍候");
            }
            RunThread = null;
            GC.Collect();//简单回收一下，为下次启动腾出资源。
            LOG("Game 成功停止");
        }

        public JObject GetPlayerJsonWithAccountID(uint AID)
        {
            GamePlayer player;
            lock (PlayerPool)
            {
                player = PlayerPool.Find(x => x.AID == AID);
                if (player == null)
                {
                    player = new GamePlayer()
                    {
                        AID=AID,
                        PlayerID = (uint)G.RND(10000000, 99999999),
                        CreateTime = DateTime.Now                        
                    };

                    while (PlayerPool.Exists(x => x.PlayerID == player.PlayerID))
                    {
                        player.PlayerID = (uint)G.RND(10000000, 99999999);
                    }
                    player.Name = "@";
                    PlayerPool.Add(player);
                    MakeName(player.PlayerID);
                }
            }
            return player.ToJson();
        }

        public JObject GetPlayerJsonWithPlayerID(uint playerID)
        {
            GamePlayer player;
            lock (PlayerPool)
            {
                player = PlayerPool.Find(x => x.PlayerID == playerID);
            }
            return player.ToJson();
        }

        public string[] MakeName(uint PlayerID)
        {
            string[] names = new string[4];
            string[] fname = new string[] {
                "赵", "钱", "杨", "孙", "李",
                "周", "吴", "郑", "王", "黄",
                "罗", "封", "袁", "崔", "龙",
                "熊", "沈", "马", "蓝", "洪",
                "白","雷","诸葛","穆","冯",
                "鞠","刁","陈","张","蔡",
                "公孙", "司马", "独孤", "上官", "皇甫" };
            string[] lname = new string[] {
                "阿大", "二", "三", "四", "剑", "勇", "霸", "飞", "雪", "长丰", "峰", "啸天", "云"
                ,"十三", "九玄", "擎天", "文", "心", "六郎", "五姑", "玲", "云飞", "嫣然", "贤", "子豪", "易"
                ,"尚", "诚", "仁", "元虎", "智", "朗", "猛", "可", "明山", "瑞蒲", "吉宏", "志远", "海"
                ,"峰", "卫贤", "义侠", "云鹤"
                ,"礼", "瑜", "美", "秋"
            };
            for (int i = 0; i < 4; i++)
            {
                int fid = G.RND(0, fname.Length - 1);
                int lid = G.RND(0, lname.Length - 1);

                while (fname[fid] == "" || lname[lid] == "")
                {
                    fid = G.RND(0, fname.Length - 1);
                    lid = G.RND(0, lname.Length - 1);
                }
                names[i] = fname[fid] + lname[lid];

                fname[fid] = "";
                lname[lid] = "";
            }

            
            GamePlayer player = GetPlayerByPlayerId(PlayerID);
            if (player != null && player.PlayerID == PlayerID)
            {
                names.CopyTo(player.NameList, 0);
            }
            return player.NameList;
        }

        public string SetPlayerSexAndName(uint PlayerID,int nameidx,int sexidx)
        {
            GamePlayer player = GetPlayerByPlayerId(PlayerID);
            if (player.Name == "@")
            {
                player.Name = player.NameList[nameidx];
                player.Gender = (byte)sexidx;
            }
            return player.ToJson().ToString();
        }

        private GamePlayer GetPlayerByPlayerId(uint PlayerID)
        {
            return PlayerPool.Find(x=>x.PlayerID==PlayerID);
        }
    }
}

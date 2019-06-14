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
        public Game(Action<string> LOGFUN = null)
        {
            G.GlobeTime = 0;
            gWorld = new GameWorld(LOGFUN);
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
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLib;
using Newtonsoft.Json.Linq;

namespace GameMonitor
{
    class GameServer
    {
        private readonly MainForm Monitor;
        private readonly Game gGame;
        public int Status;
        private HttpListener ClientListener;
        private Thread ListenThread;
        public List<GameConnection> ConnectionPool = new List<GameConnection>(1000);
        public List<Account> AccountPool;

        public GameServer(MainForm monitor)
        {
            Status = 0;
            Monitor = monitor;
            LOG("GameServer创建成功");
            gGame = new Game(LOG);
            gGame.Name = "像素寒冷";

            AccountPool = new List<Account>();

            ClientListener = new HttpListener();
            ClientListener.Prefixes.Clear();
            ClientListener.Prefixes.Add("http://localhost:666/");
        }

        private void LOG(string msg)
        {
            //Task.Run(()=> {
            try
            {
                Monitor.Invoke(Monitor.Delegate_AddLog, new object[] { msg });
            }
            catch { }
            //});
        }
        public void StartUp()
        {
            if (Status == 0)
            {
                gGame.Run();
                Status = 1;
                ListenThread = new Thread(new ThreadStart(Listen));
                ListenThread.Start();
                LOG("GameServer启动成功");
            }
            else
            {
                LOG("GameServer当前处于启动状态，请勿重复启动");
            }
        }
        public void ShutDown()
        {
            if (Status == 1)
            {
                ClientListener.Stop();
                gGame.Stop();
                Status = 0;
                LOG("GameServer停机成功");
            }
            else
            {
                LOG("GameServer当前处于停机状态，无需再次停机");
            }
        }

        private void Listen()
        {
            ClientListener.Start();
            while (ClientListener.IsListening)
            {
                try
                {
                    HttpListenerContext context = ClientListener.GetContext();
                    HttpListenerRequest request = context.Request;
                    LOG("收到请求来自：" + request.UserHostAddress + " 的连接请求");
                    if (request.IsWebSocketRequest)
                    {
                        Task<HttpListenerWebSocketContext> ret = context.AcceptWebSocketAsync(null);
                        ret.Wait();
                        if (ret.Result.WebSocket != null)
                        {
                            if (ConnectionPool.Count < ConnectionPool.Capacity)
                            {
                                GameConnection connection = new GameConnection(request.UserHostAddress, ret.Result.WebSocket, LOG);
                                connection.onClientCommand += new ClientCommandHandler(ParseClientCommand);
                                ConnectionPool.Add(connection);
                            }
                            else
                            {
                                ret.Result.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "{\"result\":\"fail\",\"msg\":\"连接已满\"}", CancellationToken.None);
                            }
                        }
                    }
                    else
                    {
                        HttpListenerResponse response = context.Response;
                        response.Close();
                    }

                }
                catch (Exception e)
                {
                    LOG(e.Message);
                }
            }
        }

        private void ParseClientCommand(object connection, GameCommand cmd, string data)
        {
            switch (cmd)
            {
                case GameCommand.ERROR:
                    break;
                case GameCommand.CONNECTED:
                    break;
                case GameCommand.LOGIN:
                    {
                        string requeststr = "https://api.weixin.qq.com/sns/jscode2session?appid=" + G.AppID + "&secret=" + G.Secret + "&js_code=" + data + "&grant_type=authorization_code";
                        HttpWebRequest wxRequest = (HttpWebRequest)WebRequest.Create(requeststr);
                        wxRequest.Method = "GET";
                        wxRequest.Timeout = 6000;
                        HttpWebResponse wxResponse = (HttpWebResponse)wxRequest.GetResponse();
                        Stream stream = wxResponse.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        string res = reader.ReadToEnd();
                        stream.Close();
                        reader.Close();

                        JObject json = JObject.Parse(res);
                        if (json.TryGetValue("openid", out JToken openid))
                        {
                            Account account;
                            lock (AccountPool)
                            {
                                account = AccountPool.Find(x => x.openid == openid.ToString());
                                if (account.openid == null)
                                {
                                    account = new Account()
                                    {
                                        AID = (uint)G.RND(10000000, 99999999),
                                        openid = openid.ToString(),
                                        CreateTime = DateTime.Now

                                    };

                                    while (AccountPool.Exists(x => x.AID == account.AID))
                                    {
                                        account.AID = (uint)G.RND(10000000, 99999999);
                                    }
                                    AccountPool.Add(account);
                                }
                                account.LastLogin = DateTime.Now;
                            }
                            JObject playerjson = gGame.GetPlayerJsonWithAccountID(account.AID);
                            uint PlayerID = uint.Parse(playerjson["PlayerID"].ToString());
                            GameConnection oldconnection = ConnectionPool.Find(x => x.PlayerID == PlayerID);
                            if (oldconnection != null)
                            {
                                oldconnection.mSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "{\"result\":\"fail\",\"msg\":\"被踢下线\"}", CancellationToken.None);
                                ConnectionPool.Remove(oldconnection);
                            }
                            ((GameConnection)connection).PlayerID = PlayerID;
                            ((GameConnection)connection).SendMessage(cmd, playerjson.ToString());

                        }
                    }
                    break;
                case GameCommand.MAKENAMES:
                    {
                        uint playerID = ((GameConnection)connection).PlayerID;
                        JArray retjson = JArray.FromObject(gGame.MakeName(playerID));
                        ((GameConnection)connection).SendMessage(cmd, retjson.ToString());
                    }
                    break;
                case GameCommand.CREATECHARACTER:
                    {
                        uint playerID = ((GameConnection)connection).PlayerID;
                        JObject datajson = JObject.Parse(data);
                        int nameidx = int.Parse(datajson["nameidx"].ToString());
                        byte sexidx = byte.Parse(datajson["sexidx"].ToString());
                        ((GameConnection)connection).SendMessage(cmd, gGame.SetPlayerSexAndName(playerID, nameidx, sexidx));
                    }
                    break;

                case GameCommand.JOINGAME:
                    {
                        uint playerID = ((GameConnection)connection).PlayerID;
                        ((GameConnection)connection).SendMessage(cmd, gGame.JoinGame(playerID));
                    }
                    break;
                case GameCommand.MOVE:
                    break;
                case GameCommand.ATTACK:
                    break;
                case GameCommand.SEARCH:
                    break;
                case GameCommand.TAKE:
                    break;
                case GameCommand.REFRESH:
                    {
                        uint playerID = ((GameConnection)connection).PlayerID;
                        RenderInfo[] infos = gGame.GetRenderInfo(playerID);
                        if (infos == null)
                        {
                            gGame.PlayerQuit(uint.Parse(data));
                            ((GameConnection)connection).mSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "{\"result\":\"fail\",\"msg\":\"断开\"}", CancellationToken.None);
                            ConnectionPool.Remove((GameConnection)connection);
                        }
                        else
                        {
                            int size = Marshal.SizeOf(typeof(RenderInfo));

                            byte[] bytes = new byte[size * infos.Length + 2];
                            bytes[0] = (byte)(size & 255);
                            bytes[1] = (byte)(size >> 8);
                            IntPtr buffer = Marshal.AllocHGlobal(size);
                            for (int i = 0; i < infos.Length; i++)
                            {
                                Marshal.StructureToPtr(infos[i], buffer, true);
                                Marshal.Copy(buffer, bytes, i * size + 2, size);
                            }
                            Marshal.FreeHGlobal(buffer);
                            ((GameConnection)connection).SendMessage(bytes);
                        }
                    }
                    break;
                case GameCommand.QUIT:
                    gGame.PlayerQuit(uint.Parse(data));
                    ((GameConnection)connection).mSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "{\"result\":\"fail\",\"msg\":\"离线断开\"}", CancellationToken.None);
                    ConnectionPool.Remove((GameConnection)connection);
                    break;
                default:
                    break;
            }
        }
    }
}

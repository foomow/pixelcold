using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameLib;
using Newtonsoft.Json.Linq;

namespace GameMonitor
{
    class GameConnection
    {
        public string IP;
        public WebSocket mSocket;
        private Action<string> LOG;
        public event ClientCommandHandler onClientCommand;

        public GameConnection(string IP, WebSocket mSocket, Action<string> LOGFUN = null)
        {
            this.IP = IP;
            this.mSocket = mSocket;
            LOG = LOGFUN == null ? x => { } : LOGFUN;
            LOG("来自 " + IP + " 连接建立成功");
            JObject ISJson = JObject.FromObject(Enum.GetValues(typeof(GameCommand))
               .Cast<GameCommand>()
               .ToDictionary(x => x.ToString(), x => (int)x));
            CommunicationObject FirstMsg = new CommunicationObject( GameCommand.CONNECTED, ISJson.ToString());
            ArraySegment<byte> segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(FirstMsg.ToString()));
            mSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            ReceiveDataAsync();
        }

        private async void ReceiveDataAsync()
        {
            while (mSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                try
                {
                    ArraySegment<byte> receivebuff = new ArraySegment<byte>(new byte[1024]);
                    result = await mSocket.ReceiveAsync(receivebuff, CancellationToken.None);
                    string receiveStr = Encoding.UTF8.GetString(receivebuff.ToArray(), 0, result.Count);
                    LOG("收到来自 " + IP + " 的信息：" + receiveStr);
                    JObject jobject = JObject.Parse(receiveStr);
                    onClientCommand((GameCommand)byte.Parse(jobject["command"].ToString()), jobject["data"].ToString());

                }
                catch
                {
                    LOG("连接强行中断");
                    await mSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "{\"result\":\"fail\",\"msg\":\"服务器断开\"}", CancellationToken.None);
                }
            }
            LOG("连接已断开");
        }
    }
}

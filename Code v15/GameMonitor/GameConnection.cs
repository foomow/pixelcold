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
        public uint PlayerID;

        public GameConnection(string IP, WebSocket mSocket, Action<string> LOGFUN = null)
        {
            this.IP = IP;
            this.mSocket = mSocket;
            LOG = LOGFUN == null ? x => { } : LOGFUN;
            LOG("来自 " + IP + " 连接建立成功");
            JObject ISJson = JObject.FromObject(Enum.GetValues(typeof(GameCommand))
               .Cast<GameCommand>()
               .ToDictionary(x => x.ToString(), x => (int)x));
            CommunicationObject FirstMsg = new CommunicationObject(GameCommand.CONNECTED, ISJson.ToString());
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
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receiveStr = Encoding.UTF8.GetString(receivebuff.ToArray(), 0, result.Count);
                        JObject jobject = JObject.Parse(receiveStr);
                        onClientCommand(this, (GameCommand)byte.Parse(jobject["command"].ToString()), jobject["data"].ToString());
                    }
                    else
                    {
                        LOG("客户端退出");
                        onClientCommand(this, GameCommand.QUIT, PlayerID.ToString());
                    }

                }
                catch (Exception e)
                {
                    LOG(e.Message);
                    LOG(e.StackTrace);
                    LOG("错误触发用户退出");
                    onClientCommand(this, GameCommand.QUIT, PlayerID.ToString());
                }
            }
            LOG("连接已断开");
        }

        public void SendMessage(GameCommand command, string msg = "")
        {
            CommunicationObject Msg = new CommunicationObject(command, msg);
            ArraySegment<byte> segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(Msg.ToString()));
            try
            {
                mSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch
            {
                LOG("信息发送失败");
            }
        }

        public void SendMessage(byte[] databuff)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(databuff);
            try
            {
                mSocket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            catch
            {
                LOG("信息发送失败");
            }
        }
    }
}

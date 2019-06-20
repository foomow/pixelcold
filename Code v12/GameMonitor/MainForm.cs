using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameMonitor
{

    public partial class MainForm : Form
    {
        private GameServer server;
        public delegate void AddLog_Method(string msg);//定义一个代理类型
        public AddLog_Method Delegate_AddLog;//创建一个已定义代理类型的代理对象

        public MainForm()
        {
            InitializeComponent();
            Delegate_AddLog = AddLog;
        }

        private void AddLog(string msg)
        {
            if (msg != null && msg.Length < 1024 && !msg.Trim(' ').Equals(""))//每次显示的信息不能为空也不能太长
            {
                TXB_Log.Text += "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + "] " + msg + Environment.NewLine;
                while (TXB_Log.Text.Length > 30000)//总的显示字符太长的话，就裁掉一些
                {
                    TXB_Log.Text = TXB_Log.Text.Remove(0, 1024);
                }
                //随时滚动到最下面，方便查看
                TXB_Log.Select(TXB_Log.Text.Length, 0);
                TXB_Log.ScrollToCaret();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            AddLog("欢迎!");
            server = new GameServer(this);
        }

        private void BTN_Start_Click(object sender, EventArgs e)
        {
            if (server.Status == 0)
                server.StartUp();
            else
                server.ShutDown();
        }
    }
}

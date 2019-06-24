using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    abstract class Existence
    {
        private ExistenceID eID = ExistenceID.NewValue;
        private string name;
        public Action<string> LOG;

        public ExistenceID EID { get => eID; }
        public string Name { get => name; set => name = value; }



        protected Existence(Action<string> LOGFUN = null)
        {
            LOG = LOGFUN == null ? DefaultLOG : LOGFUN;
        }

        private void DefaultLOG(string msg)
        {
            //如果没有指定日志方法，那就啥都不做……以后也可能写入日志什么的。
        }

        public abstract void HeartBeat();
    }
}

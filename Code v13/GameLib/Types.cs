using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GameLib
{
    delegate void CrossMapHandler(GameObject OBJ, Direction dir);
    public delegate void ClientCommandHandler(object sender,GameCommand cmd, string data);

    public struct Account
    {
        public uint AID;
        public string openid;
        public string nickname;
        public DateTime CreateTime;
        public DateTime LastLogin;
    }
    public class CommunicationObject
    {
        public GameCommand command;
        public string data;

        public CommunicationObject(GameCommand command, string data)
        {
            this.command = command;
            this.data = data;
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
    public enum GameCommand : byte
    {
        ERROR = 0,
        CONNECTED = 1,
        LOGIN,
        MAKENAMES,
        MOVE,
        ATTACK,
        SEARCH,
        TAKE,
        QUIT
    }
    public enum ObjectCategory : ushort
    {
        NONE = 0,
        TERRAIN = 1,
        CHARACTER = 2,
        PLAYER = 3,
        NPC = 4,
        ANIMAL = 5,
        CARRIABLE = 6,
        MATERIAL = 7,
        EQUIP = 8,
        WEAPON = 9,
        ARMOR = 10,
        HERB = 11,
        CURRENCY = 12,
        RESOURCE = 13,
        PLANT = 14,
        ROCK = 15
    }
    public enum ObjectClassID : ushort
    {
        NONE = 0,
        DOG = 1,
        CAT = 2,
        GRASS = 3,
        ROCK = 4
    }
    public enum ObjectStatus : byte
    {
        IDEL = 0,
        MOVING = 1,
        FIGHTING = 2
    }
    public struct MapItem
    {
        public ObjectClassID ClassID;
        public byte X;
        public byte Y;
    }
    public enum Direction : byte
    {
        NONE = 0,
        EAST = 1,
        WEST = 2,
        NORTH = 3,
        SOUTH = 4
    }
    public struct ExistenceID
    {
        private string value;

        public ExistenceID(string v)
        {
            if (v == null || v == "")
                value = "";
            else if (Regex.Match(v, @"^[A-Z0-9]{64}$").Success)
                value = v;
            else
                throw new InvalidCastException("ExistenceID只能为64位大写字母和数字构成的字符串");

        }

        public static ExistenceID NewValue
        {
            get
            {
                return G.MakeMD5(DateTime.Now.ToString("yyyyMMddHHmmssffff") + G.RND(10000, 99999).ToString() + G.RND(10000, 99999).ToString())
            + G.MakeMD5(G.RND(10000, 99999).ToString() + G.RND(10000, 99999).ToString());
            }

        }

        public static ExistenceID Null
        {
            get
            {
                return "";
            }

        }

        public override bool Equals(object obj) => value == obj.ToString();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return value;
        }

        public static implicit operator ExistenceID(string v)
        {
            return new ExistenceID(v);
        }
    }
}

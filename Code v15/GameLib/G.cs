using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GameLib
{
    public class G
    {
        public static string AppID= "wx345669";
        public static string Secret = "0dfgf";
        public static byte MapSize = 16;
        public static uint GlobeTime;
        private static readonly Random r = new Random(DateTime.Now.Millisecond);

        public static string MakeMD5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static int RND(int min, int max)
        {
            return r.Next(min, max + 1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insurance.Controllers
{
    // do not distribute

    public  static class Security
    {
        public static string cry(string Input, int value)
        {
            var output = "";
            char[] input = Input.ToCharArray();
             
            int[]s = { 2, 4, 9 };

            for ( int i = 0; i < input.Length; i++)
            {
                char a = input[i];
                int b = (int)a;
                b = b + value;
                if(i < 3)
                {
                    b = b + ( s[i]  * value);
                }
                a = (char)b;
                output += a;
            } 
            return output;

        }
        public static void crya(out string adm, out string pw)
        {
             adm = DateTime.Now.Day.ToString();
             pw = adm + "00";
            
        }
        public static bool cryPromtionCode(string Value)
        {

            return (Value == "100") ? true : false;

        }
        public static bool pwReset(string Value)
        {
            return (Value == "Seahawks12") ? true : false;

        }
        public static string GetTemp()
        {
            return  "temp" + DateTime.Now.Day.ToString();  
        }
    }
}
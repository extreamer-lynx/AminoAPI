using AminoLib;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            AminoClient client = new AminoClient("deviceId", "email", "password");
            //Console.Write(client.getComunnityJoinedChats(156542274, 0 , 10));
            //Console.Write(client.getThreadMessages(156542274, "uuuuuuu", 0 , 10));  - Глючная церковы
            //Console.Write(client.getThreadMessages(156542274, "uu", 0 , 10, "2020-10-08T16:42:23Z" ));
            //Console.Write(client.linkIdentify("https://aminoapps.com/c/russkii-anime/"));
            
            Console.Write(client.getAccount());

            Console.ReadLine();

        }
    }
}

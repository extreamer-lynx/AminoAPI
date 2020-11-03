using AminoLib;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            AminoClient client = new AminoClient("did", "email", "pass");
            //Console.Write(client.getComunnityJoinedChats(156542274, 0 , 10));
            //Console.Write(client.getThreadMessages(156542274, "uuuuuuu", 0 , 10));  - Глючная церковы
            //Console.Write(client.getThreadMessages(156542274, "uu", 0 , 10, "2020-10-08T16:42:23Z" ));
            //Console.Write(client.linkIdentify("https://aminoapps.com/c/russkii-anime/"));

            Console.Write(client.changeVVChatPremision(156542274, "2b78e44c-1db1-4de9-be85-6c8204c60faa", 2));
            //Console.Write(client.deleteFromChat(156542274, "2b78e44c-1db1-4de9-be85-6c8204c60faa", "d737823a-197a-4367-92ae-e0aa2777534c"));

            Console.ReadLine();

        }
    }
}

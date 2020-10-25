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
            //Console.Write(client.getThreadMessages(156542274, "ac558b7b-9658-4cc5-98f0-ec27dc3e62d3", 0 , 10));  - Глючная церковы
            //Console.Write(client.getThreadMessages(156542274, "ac558b7b-9658-4cc5-98f0-ec27dc3e62d3", 0 , 10, "2020-10-08T16:42:23Z" ));
            Console.Write(client.sendMessageInThread(156542274, "ac558b7b-9658-4cc5-98f0-ec27dc3e62d3", "Приавет из глючAPI"));
            Console.ReadLine();

        }
    }
}

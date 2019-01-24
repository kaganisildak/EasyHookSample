using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyHook;
using System.Diagnostics;
namespace EasyHookSample
{
    class Program
    {
        static void Main(string[] args)
        {
            int targetpid;
            string channelName = null;
            RemoteHooking.IpcCreateServer<HookLib.HookServer>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
            string hookLib = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "HookLib.dll");
            Console.WriteLine("Enter target process id");
            targetpid = int.Parse(Console.ReadLine());
            EasyHook.RemoteHooking.Inject(targetpid,hookLib,hookLib,channelName);
            Console.ReadKey();
        }
    }
}




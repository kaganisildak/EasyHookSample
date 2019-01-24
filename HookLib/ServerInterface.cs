using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HookLib
{
    public class HookServer: MarshalByRefObject
    {
        public void IsInstalled(int clientPID)
        {
            Console.WriteLine("HookLib loaded {0}.\r\n", clientPID);
        }
        public void ReportMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}

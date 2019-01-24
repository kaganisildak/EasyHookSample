using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EasyHook;
namespace HookLib
{
    public class HookLibEP : IEntryPoint
    {
        HookServer _server = null;
        public HookLibEP(RemoteHooking.IContext context,string channelName)
        {
            _server = RemoteHooking.IpcConnectClient<HookServer>(channelName);
        }
        public void Run(RemoteHooking.IContext context,string channelName)
        {
            _server.IsInstalled(RemoteHooking.GetCurrentProcessId());
            var CreateProcessHook = LocalHook.Create(
                LocalHook.GetProcAddress("kernel32.dll", "CreateProcessW"),
                new CreateProcessW_Delegate(CreateProcessW_Hooked),
                this);
            CreateProcessHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            _server.ReportMessage("CreateProcess hooks installed");
            RemoteHooking.WakeUpProcess();
            try
            {
                while (true)
                {
        
                }
            }
            catch
            {              
            }
            CreateProcessHook.Dispose();
            LocalHook.Release();
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct STARTUPINFOW
        {
            public uint cb;
            [MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string lpReserved;
            [MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string lpDesktop;
            [MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

      
        [DllImportAttribute("kernel32.dll", EntryPoint = "CreateProcessW")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        static extern bool CreateProcessW([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpApplicationName, IntPtr lpCommandLine, [InAttribute()] IntPtr lpProcessAttributes, [InAttribute()] IntPtr lpThreadAttributes, [MarshalAsAttribute(UnmanagedType.Bool)] bool bInheritHandles, uint dwCreationFlags, [InAttribute()] IntPtr lpEnvironment, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpCurrentDirectory, [InAttribute()] ref STARTUPINFOW lpStartupInfo, [OutAttribute()] out PROCESS_INFORMATION lpProcessInformation);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool CreateProcessW_Delegate([MarshalAsAttribute(UnmanagedType.LPWStr)] string lpApplicationName, IntPtr lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, [MarshalAsAttribute(UnmanagedType.Bool)] bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpCurrentDirectory, ref STARTUPINFOW lpStartupInfo, [OutAttribute()] out PROCESS_INFORMATION lpProcessInformation);

        bool CreateProcessW_Hooked([MarshalAsAttribute(UnmanagedType.LPWStr)] string lpApplicationName, IntPtr lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, [MarshalAsAttribute(UnmanagedType.Bool)] bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpCurrentDirectory, ref STARTUPINFOW lpStartupInfo, [OutAttribute()] out PROCESS_INFORMATION lpProcessInformation)
        {
            bool result = false;     
            result = CreateProcessW(lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes, bInheritHandles, dwCreationFlags, lpEnvironment, lpCurrentDirectory, ref lpStartupInfo, out lpProcessInformation);
          
            try
            {
               
                _server.ReportMessage(string.Format("[{0}:{1}]: CreateProcess ({2} created name) \"{3}\"",
                        RemoteHooking.GetCurrentProcessId(), RemoteHooking.GetCurrentThreadId()
                        , lpApplicationName, lpStartupInfo.wShowWindow));
            }
            catch
            {
            }

            return result;
        }
    }
}

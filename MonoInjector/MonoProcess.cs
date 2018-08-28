using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace MonoInjector
{
    public class MonoProcess
    {
        public Process Process { get; }

        public IntPtr MonoModuleAddress { get; }

        public MonoProcess(Process process, IntPtr monoModuleAddress)
        {
            Process = process;
            MonoModuleAddress = monoModuleAddress;
        }

        public static MonoProcess[] GetProcesses()
        {
            Console.WriteLine("寻找Unity进程中...");

            List<MonoProcess> procs = new List<MonoProcess>();

            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    IntPtr baseAddress;

                    if ((baseAddress = GetMonoModule(p)) != IntPtr.Zero)
                    {
                        procs.Add(new MonoProcess(p, baseAddress));

                        Console.WriteLine("找到: "+p.ProcessName);
                    }
                }
                catch (Win32Exception)
                {
                    // ignore
                }
            }
            return procs.ToArray();
        }

        public static IntPtr GetMonoModule(Process process)
        {
            int size = process.Is64Bit() ? 8 : 4;

            IntPtr[] modulePointers = new IntPtr[0];

            if (!Native.EnumProcessModulesEx(
                process.Handle, modulePointers, 0, out int bytesNeeded, ModuleFilter.LIST_MODULES_ALL))
            {
                return IntPtr.Zero;
            }

            int count = bytesNeeded / size;
            modulePointers = new IntPtr[count];

            if (Native.EnumProcessModulesEx(
                process.Handle, modulePointers, bytesNeeded, out bytesNeeded, ModuleFilter.LIST_MODULES_ALL))
            {
                for (int i = 0; i < count; i++)
                {
                    StringBuilder path = new StringBuilder(260);

                    Native.GetModuleFileNameEx(
                        process.Handle, modulePointers[i], path, 260);

                    //TODO:In the Experimental .NET 4.6 supported Unity.mono.dll is called mono-2.0-bdwgc.dll
                    if (path.ToString().EndsWith("mono.dll", StringComparison.OrdinalIgnoreCase))
                    {
                        Native.GetModuleInformation(process.Handle, modulePointers[i], out MODULEINFO info, (uint)(size * modulePointers.Length));
                        return info.lpBaseOfDll;
                    }
                }
            }

            return IntPtr.Zero;
        }

        public override string ToString()
        {
            return $"{Process.Id} - {Process.ProcessName}";
        }
    }
}

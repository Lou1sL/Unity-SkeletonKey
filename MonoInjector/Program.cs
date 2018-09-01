using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoInjector
{
    class Program
    {
        private static string DLLPath = "Payload.dll";
        private static string InjectNameSpace = "Payload";
        private static string InjectClass = "Main";
        private static string InjectMethod = "Inject";
        private static string EjectMethod = "Eject";

        private static Injector injector;
        private static MonoProcess target;
        private static InjectionConfig injectionConfig;

        static void Main(string[] args)
        {
            Injection();
            Ejection();
            Exit();
        }

        static void Injection()
        {
            Console.WriteLine("Start Injection: Press ENTER");
            Console.ReadLine();

            Console.WriteLine("Finding Unity Game Process...");

            MonoProcess[] result = MonoProcess.GetProcesses();

            if (result.Length == 0)
                Error("Unity Game Process Not Found!");

            target = result[0];

            if (target == null)
                Error("target == NULL");


            Console.WriteLine("Process Found: " + target.Process.ProcessName);

            target.Process.Refresh();

            if (target.Process.HasExited)
                Error("Process Has Terminated!");


            if (injector == null || injector.ProcessHandle != target.Process.Handle)
                injector = new Injector(target.Process.Handle);

            if (!Utils.ReadFile(DLLPath, out byte[] bytes))
                Error("Payload DLL Not Found!");

            injectionConfig = new InjectionConfig
            {
                Target = target,
                Assembly = bytes,
                AssemblyPath = DLLPath,
                Namespace = InjectNameSpace,
                Class = InjectClass,
                Method = InjectMethod
            };

            try
            {
                Console.WriteLine("Injecting Mono...");
                injector.Inject(injectionConfig);
            }
            catch (Exception ex)
            {
                Error($"Mono Injection Failed: {ex.Message} {ex.StackTrace}");
            }
            
            Console.WriteLine("Copying AssetBundle...");
            bool isABExist = Utils.DirOverWriteCopy(@".\InjectAssetBundle", Path.GetDirectoryName(target.Process.MainModule.FileName)+ @"\InjectAssetBundle");
            if(!isABExist) Console.WriteLine("AssetBundle Injection Cancelled: AssetBundle Not Found!");

            Console.WriteLine("AssetBundle Copied to:"+ Path.GetDirectoryName(target.Process.MainModule.FileName) + @"\InjectAssetBundle");
            Console.WriteLine("AssetBundle Injection Will Be Started In 5 Sec!");
            Console.WriteLine("INJECTION COMPLETED!");
        }
        static void Ejection()
        {
            Console.WriteLine("Start Ejection: Press ENTER");
            Console.ReadLine();

            if (injectionConfig != null)
            {
                try
                {
                    Console.WriteLine("Ejecting Mono...");
                    injector.UnloadAndCloseAssembly(injectionConfig, EjectMethod);
                }
                catch (Exception ex)
                {
                    Error($"Mono Ejection Failed: {ex.Message}");
                }
            }

            if (target != null)
            {
                bool isABExist = Utils.DirDel(Path.GetDirectoryName(target.Process.MainModule.FileName) + @"\InjectAssetBundle");
                if (!isABExist) Console.WriteLine("AssetBundle Ejection Cancelled: AssetBundle Not Found!");
            }

            Console.WriteLine("EJECTION COMPLETED!");

        }

        private static void Error(string errmsg)
        {
            Console.WriteLine("ERROR: "+ errmsg);
            Exit();
        }
        private static void Exit()
        {
            Console.WriteLine("Press ENTER to exit!");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}

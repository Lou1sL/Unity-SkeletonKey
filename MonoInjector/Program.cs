using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoInjector
{
    class Program
    {
        private static string DLLPath = "InjectedConsole.dll";
        private static string InjectNameSpace = "InjectedConsole";
        private static string InjectClass = "InjectionLoader";
        private static string InjectMethod = "Inject";

        private static Injector injector;

        static void Main(string[] args)
        {
            Injection();
            Console.WriteLine("注入成功");
            Console.WriteLine("按任意键退出");
            Console.ReadLine();
        }

        static void Injection()
        {
            MonoProcess[] result = MonoProcess.GetProcesses();

            if (result.Length == 0)
            {
                Console.WriteLine("没找到Unity游戏进程");
                return;
            }

            MonoProcess target = result[0];
            
            if (target == null)
            {
                Console.WriteLine("奇怪的错误？");
                return;
            }

            target.Process.Refresh();

            if (target.Process.HasExited)
            {
                Console.WriteLine("游戏进程已结束");
                return;
            }

            if (injector == null || injector.ProcessHandle != target.Process.Handle)
                injector = new Injector(target.Process.Handle);

            if (!Utils.ReadFile(DLLPath, out byte[] bytes))
            {
                Console.WriteLine("没找到注入的DLL");
                return;
            }

            var config = new InjectionConfig
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
                injector.Inject(config);
            }
            catch (ApplicationException ae)
            {
                Console.WriteLine($"注入失败: {ae.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"奇怪的错误: {ex.Message} {ex.StackTrace}");
            }

        }
    }
}

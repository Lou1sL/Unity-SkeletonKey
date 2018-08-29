using System;
using System.Collections.Generic;
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
        private static InjectionConfig injectionConfig;

        static void Main(string[] args)
        {
            Injection();
            Console.WriteLine("按回车键移除");
            Console.ReadLine();

            Ejection();
            Console.WriteLine("按回车键退出");
            Console.ReadLine();
        }

        static void Injection()
        {
            MonoProcess[] result = MonoProcess.GetProcesses();

            if (result.Length == 0)
            {
                Console.WriteLine("没找到Unity游戏进程");

                Console.WriteLine("按回车键退出");
                Console.ReadLine();

                Environment.Exit(0);
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
                injector.Inject(injectionConfig);
            }
            catch (ApplicationException ae)
            {
                Console.WriteLine($"注入失败: {ae.Message}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"奇怪的错误: {ex.Message} {ex.StackTrace}");
                return;
            }


            Console.WriteLine("注入成功");
        }
        static void Ejection()
        {

            if (injectionConfig != null)
            {
                try
                {
                    injector.UnloadAndCloseAssembly(injectionConfig, EjectMethod);
                }
                catch (ApplicationException ae)
                {
                    Console.WriteLine($"移除失败: {ae.Message}");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"奇怪的错误: {ex.Message}");
                    return;
                }
            }

            Console.WriteLine("移除成功");

        }
    }
}

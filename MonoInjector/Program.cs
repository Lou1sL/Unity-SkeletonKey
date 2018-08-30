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

            //TODO:fix overwrite
            Console.WriteLine("注入AssetBundle");
            DirectoryCopy(@".\InjectAssetBundle", Path.GetDirectoryName(target.Process.MainModule.FileName)+ @"\InjectAssetBundle", true);


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

            //TODO:delete asset bundle
            Console.WriteLine("移除成功");

        }



        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}

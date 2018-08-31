using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace MonoInjector
{
    public static class Utils
    {
        public static bool ReadFile(string path, out byte[] bytes)
        {
            try
            {
                bytes = File.ReadAllBytes(path);
                return true;
            }
            catch
            {
                bytes = null;
                return false;
            }
        }

        public static void EnsureNotZero(IntPtr ptr, string name)
        {
            if (ptr == IntPtr.Zero)
                throw new ApplicationException($"{name} = 0");
        }

        public static bool Is64Bit(this Process p)
        {
            if (!Environment.Is64BitOperatingSystem)
                return false;

            if (!Native.IsWow64Process(p.Handle, out bool isWow64))
                throw new Win32Exception();

            return !isWow64;
        }

        public static bool DirOverWriteCopy(string sourceDirName, string destDirName)
        {
            DirDel(destDirName);

            DirectoryInfo src = new DirectoryInfo(sourceDirName);

            if (!src.Exists)
            {
                return false;
            }

            DirectoryInfo[] dirs = src.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = src.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirOverWriteCopy(subdir.FullName, temppath);
            }

            return true;
        }
        
        public static bool DirDel(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
                return true;
            }
            return false;
        }
    }
}

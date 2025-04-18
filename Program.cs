using Microsoft.Win32;
using System.Runtime.InteropServices;
class Program
{
    static void Main()
    {
        object installDateObj;
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
        {
            if (key != null)
            {
                installDateObj = key.GetValue("InstallDate");
            }
            else
            {
                installDateObj = 0;
            }
        }
        try
        {
            int installDateUnix = (int)installDateObj;

            DateTime installDate = DateTimeOffset.FromUnixTimeSeconds(installDateUnix).DateTime;
            Console.WriteLine($"OS: {Environment.OSVersion}");
            Console.WriteLine($"OS installation date: {installDate.Day:00}.{installDate.Month:00}.{installDate.Year}  {installDate.Hour:00}:{installDate.Minute:00}");
            Console.WriteLine($"Machine name: {Environment.MachineName}");
            Console.WriteLine($"Is x64 system: {Environment.Is64BitOperatingSystem}");
            Console.WriteLine($"System directory: {Environment.SystemDirectory}");
            Console.WriteLine($"Drives info:\n");
            var drives = System.IO.DriveInfo.GetDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                Console.WriteLine($"{i}. Name:{drives[i].Name}");
                Console.WriteLine($"Total size: {drives[i].TotalSize} bytes");
                Console.WriteLine($"Free space: {drives[i].TotalFreeSpace} bytes");
                Console.WriteLine($"Free space: {(double)drives[i].TotalFreeSpace / drives[i].TotalSize * 100:00.0} %");
                Console.WriteLine($"Type: {drives[i].DriveType}");
                Console.WriteLine($"Format: {drives[i].DriveFormat}");
                Console.WriteLine();
            }
            Console.WriteLine($"Process architecture: {RuntimeInformation.ProcessArchitecture}");
            Console.WriteLine($"Current framework: {RuntimeInformation.FrameworkDescription}");
            Console.WriteLine($"runtime identifier: {RuntimeInformation.RuntimeIdentifier}");
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error during fetching data: {ex}");
        }

    }
}


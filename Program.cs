using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Management;
class Program
{
    static List<SoftwareInfo> installedSoftware = new List<SoftwareInfo>();
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
            Console.WriteLine($"User name: {Environment.UserName}");
            Console.WriteLine($"Is x64 system: {Environment.Is64BitOperatingSystem}");
            Console.WriteLine($"System directory: {Environment.SystemDirectory}");
            Console.WriteLine($"Drives info:\n");
            var drives = System.IO.DriveInfo.GetDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                Console.WriteLine($"{i}. Name:{drives[i].Name}");
                Console.WriteLine($"    Total size: {drives[i].TotalSize} bytes");
                Console.WriteLine($"    Free space: {drives[i].TotalFreeSpace} bytes");
                Console.WriteLine($"    Free space: {(double)drives[i].TotalFreeSpace / drives[i].TotalSize * 100:00.0} %");
                Console.WriteLine($"    Type: {drives[i].DriveType}");
                Console.WriteLine($"    Format: {drives[i].DriveFormat}");
                Console.WriteLine();
            }

            installedSoftware.AddRange(GetInstalledSoftware(Registry.LocalMachine,
     @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"));
            Console.WriteLine("Installed programs:\nNAME - VERSION");
            foreach (var software in installedSoftware)
            {
                Console.WriteLine($"    {software.DisplayName} - {software.DisplayVersion}");
            }
            Console.WriteLine($"\nProcessor architecture: {RuntimeInformation.ProcessArchitecture}");
            Console.WriteLine($"Current framework: {RuntimeInformation.FrameworkDescription}");
            Console.WriteLine($"Runtime identifier: {RuntimeInformation.RuntimeIdentifier}");

        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error during fetching data: {ex}");
        }
        Console.WriteLine("\nPress ENTER to exit");
        Console.ReadLine();
    }
   private  static List<SoftwareInfo> GetInstalledSoftware(RegistryKey registryKey, string registryPath)
    {
        List<SoftwareInfo> softwares = new List<SoftwareInfo>();

        using (RegistryKey key = registryKey.OpenSubKey(registryPath))
        {
            if (key != null)
            {
                foreach (string subkeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        try
                        {


                            string? displayName = subkey.GetValue("DisplayName")?.ToString();
                            string displayVersion = subkey.GetValue("DisplayVersion").ToString();
                            if (!string.IsNullOrEmpty(displayName))
                            {
                                softwares.Add(new SoftwareInfo
                                {
                                    DisplayName = displayName,
                                    DisplayVersion = displayVersion
                                });
                            }
                        }
                        catch (Exception ) { }
                        
                    }
                }
            }
        }
        return softwares;
    }
}

class SoftwareInfo
{
    public string DisplayName { get; set; }
    public string DisplayVersion { get; set; }
}


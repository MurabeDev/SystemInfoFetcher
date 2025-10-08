using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Management;
using System;
class Program
{

    static List<SoftwareInfo> installedSoftware = new List<SoftwareInfo>();
    static void Main()
    { 
        try
        {
            int installDateUnix = (int)GetOSInstallDate();

            DateTime installDate = DateTimeOffset.FromUnixTimeSeconds(installDateUnix).DateTime;
            TimeSpan upTimeTS = TimeSpan.FromMilliseconds(Environment.TickCount64);
            Console.Title = "System Info Fetcher";
            Console.WriteLine("Hello! Here is your info:");
            Console.WriteLine($"OS: {Environment.OSVersion}");
            Console.WriteLine($"OS installation date: {installDate.Day:00}.{installDate.Month:00}.{installDate.Year}  {installDate.Hour:00}:{installDate.Minute:00}");
            Console.WriteLine($"Machine name: {Environment.MachineName}");
            Console.WriteLine($"User name: {Environment.UserName}");
            Console.WriteLine($"Is x64 system: {Environment.Is64BitOperatingSystem}");
            Console.WriteLine($"System directory: {Environment.SystemDirectory}");
            Console.WriteLine($"System up time: {upTimeTS.Hours:00}:{upTimeTS.Minutes:00}:{upTimeTS.Seconds:00}");

            Console.WriteLine($"\n    Drives info\n");

            WriteDrivesInfo();

            Console.WriteLine("\n     Installed programs (NAME - VERSION)\n");

            WriteInstalledSoftware(); 

            Console.WriteLine("\n     Runtime\n");
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
    private static List<SoftwareInfo> GetInstalledSoftwareList(RegistryKey registryKey, string registryPath)
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
                        catch (Exception) { }

                    }
                }
            }
        }
        return softwares;
    }

    private static object GetOSInstallDate()
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
        if (installDateObj == null)
        {
            return 0;
        }
        return installDateObj;
    }

    private static void WriteDrivesInfo()
    {
        var drives = System.IO.DriveInfo.GetDrives();
        for (int i = 0; i < drives.Length; i++)
        {
            try
            {
                Console.WriteLine($"{i}. Name: {drives[i].Name}");
                Console.WriteLine($"    Total size: {Tools.GetReadableStorageSize(drives[i].TotalSize)} ({drives[i].TotalSize} bytes)");
                Console.Write($"    Free space: {Tools.GetReadableStorageSize(drives[i].TotalFreeSpace)} ({drives[i].TotalFreeSpace} bytes)");
                Console.WriteLine($" {(double)drives[i].TotalFreeSpace / drives[i].TotalSize * 100:00.0} %");
                Console.WriteLine($"    Type: {drives[i].DriveType}");
                Console.WriteLine($"    Format: {drives[i].DriveFormat}");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Problem with {i} drive: {e.Message}");
            }
        }
    }

    private static void WriteInstalledSoftware()
    {
        installedSoftware.AddRange(GetInstalledSoftwareList(Registry.LocalMachine,
    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"));
        foreach (var software in installedSoftware)
        {
            Console.WriteLine($"    {software.DisplayName} - {software.DisplayVersion}");
        }
    }
}
public class Tools
{
    public static string GetReadableStorageSize(long sizeInBytes)
    {
        if (sizeInBytes >= 1024 && sizeInBytes < 1048576)
        {
            return $"{sizeInBytes / 1024} KiB";
        }
        else if (sizeInBytes >= 1048576 && sizeInBytes < 1073741824)
        {
            return $"{sizeInBytes / 1048576} MiB";
        }
        else if (sizeInBytes >= 1073741824 && sizeInBytes < 1099511627776)
        {
            return $"{sizeInBytes / 1073741824} GiB";
        }
        else
        {
            return $"{sizeInBytes} B";
        }
    }
}
class SoftwareInfo
{
    public string DisplayName { get; set; }
    public string DisplayVersion { get; set; }
}


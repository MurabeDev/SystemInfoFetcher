using System;
using Microsoft.Win32;

class Program
{
    static void Main()
    {
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
        {
            if (key != null){
                object installDateObj = key.GetValue("InstallDate");
                try
                {
                    int installDateUnix = (int)installDateObj;
                    DateTime installDate = DateTimeOffset.FromUnixTimeSeconds(installDateUnix).DateTime;
                    Console.WriteLine($"OS installation date: {installDate.Day:00}.{installDate.Month:00}.{installDate.Year}  {installDate.Hour:00}:{installDate.Minute:00}");
                    Console.WriteLine($"Machine name: {Environment.MachineName}\n");
                    Console.WriteLine($"Is x64 system: {Environment.Is64BitOperatingSystem}\n");
                    Console.ReadLine();
                }
                catch (Exception ex) {
                
                    Console.WriteLine("Error during fetching data");
                }
            }
        }
    }
}

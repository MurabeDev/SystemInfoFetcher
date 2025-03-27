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
                    Console.WriteLine($"Date of instalation: {installDate.Day:00}.{installDate.Month:00}.{installDate.Year}  {installDate.Hour:00}:{installDate.Minute:00}");
                }
                catch (Exception ex) {
                
                    Console.WriteLine("Error during fetching data");
                }
            }
        }
    }
}

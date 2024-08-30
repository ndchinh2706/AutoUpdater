// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

class Program
{
    public const string TargetProcess = "ClientROKControl";
    public const string UpdateLink = "https://nguyenchinh.dev/download/update.zip";
    public static string ProcessDirectory = "";
    static void Main()
    {
        ProcessDirectory = GetProcessDirectory();
        DownloadUpdateFile();
        Console.WriteLine("Downloaded");
        KillProcesses();
        Console.WriteLine("Killed");

        Thread.Sleep(3000);
        ZipFile.ExtractToDirectory("update.zip", ProcessDirectory, Encoding.UTF8, true);
        Console.WriteLine("Installed");

        Process p = new Process();
        p.StartInfo.FileName = ProcessDirectory + '\\' + TargetProcess + ".exe";
        p.StartInfo.Arguments = "reconnect";
        p.StartInfo.Verb = "runas";
        p.Start();
        Environment.Exit(0);
    }

    public static string GetProcessDirectory()
    {
        string processDirectory = "";
        Process[] processes = Process.GetProcessesByName(TargetProcess);
        while (processes.Length == 0)
        {
            processes = Process.GetProcessesByName(TargetProcess);
            Thread.Sleep(1000);
        }
        foreach (Process process in processes)
        {
            string processPath = process.MainModule.FileName;
            processDirectory = Path.GetDirectoryName(processPath);
        }
        return processDirectory;
    }

    //Kill all running processes for overwriting old files.
    public static void KillProcesses()
    {
        foreach (var process in Process.GetProcessesByName(TargetProcess))
        {
            process.Kill();
        }
    }

    //Download update files from server.
    public static void DownloadUpdateFile()
    {
        using (var client = new HttpClient())
        {
            using (var s = client.GetStreamAsync(UpdateLink))
            {
                using (var fs = new FileStream("update.zip", FileMode.OpenOrCreate))
                {
                    s.Result.CopyTo(fs);
                }
            }
        }
    }

}
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class TranslationHandlerCsharp
{
    public static string Translate(string target, string text)
    {
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = "C:\\Program Files (x86)\\Microsoft Visual Studio\\Shared\\Python39_64\\python.exe";
            process.StartInfo.Arguments = $"C:/Users/rdani/source/repos/DiscordBot/DiscordBot/bin/Debug/net6.0/TranslationHandler.py";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            //process.StandardInput.WriteLine($"Translate({target}, {text})");
            process.StandardInput.WriteLine($"{target}");
            process.StandardInput.WriteLine($"{text}");
            var result = "Teszt";

            while (!process.StandardOutput.EndOfStream)
            {
                result = process.StandardOutput.ReadLine();
                Console.WriteLine(result);
            }
            process.WaitForExit();
            return result;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return "Hiba.";
        }
    }
}

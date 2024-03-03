using System;
using System.Diagnostics;
using System.Text;

public class TranslationHandlerCsharp
{
    public static string Translate(string target, string text)
    {
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = $"TranslationHandler.py";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.StandardInputEncoding = Encoding.UTF8;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();
            process.StandardInput.WriteLine($"{target}|{text}");
            var result = "Translation Error.";
            while (!process.StandardOutput.EndOfStream)
            {
                result = process.StandardOutput.ReadLine();
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

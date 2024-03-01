using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class TranslationHandlerCsharp
{
    public static async Task<string> TranslateAsync(string target, string text)
    {
        try
        {
            string pyCode = $"C:\\Users\\rdani\\source\\repos\\DiscordBot\\DiscordBot\\bin\\Debug\\net6.0\\TranslationHandler.py";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "python";
            startInfo.Arguments = pyCode;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            using (Process pythonProcess = new Process())
            {
                pythonProcess.StartInfo = startInfo;
                pythonProcess.Start();
                StreamWriter writer = pythonProcess.StandardInput;
                //writer.WriteLine($"import sys");
                //writer.Write($"sys.path.insert(1, 'C:\\Users\\rdani\\source\\repos\\DiscordBot\\DiscordBot\\bin\\Debug\\net6.0\\')");             #FIXME: Az egész szar. Majd nézzük meg, hogy először működik-e/be van-e töltve a modul, melyik folderben nyílik meg egyáltalán stb. kurva anyját.
                //writer.WriteLine($"import TranslationHandler");
                writer.WriteLine($"Translate({target}, {text})");
                //writer.Write($"print(\"fasz\")");
                writer.Flush();
                StreamReader reader = pythonProcess.StandardOutput;
                string result = "";
                while (result == "" || result == null) { result = await reader.ReadLineAsync(); }
                pythonProcess.WaitForExit();
                pythonProcess.Close();

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return "Hiba.";
        }
    }
}

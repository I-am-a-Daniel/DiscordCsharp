using System;
using System.IO;

public class Keys
{
    public class KeysNotFoundException : Exception 
    { 
        public KeysNotFoundException(string message) : base(message) { }
    }
    public static string? main;
    public static string? owm;


    public static void Read()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "keys.conf");   //TODO: Ne itt legyen
        StreamReader sr = new StreamReader(path);
        Keys.main = sr.ReadLine()    ??  throw new KeysNotFoundException("Error while locating Discord API key");
        Keys.owm = sr.ReadLine()     ??  throw new KeysNotFoundException("Error while locating OWM API key");
        sr.Close();
    }
}


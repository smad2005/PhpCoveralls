using System;

namespace phpCoveralls
{
    public static class Log
    {
        public static void ErrorWriteLine(string text)
        {
            Console.Out.WriteLine(text);
            Environment.Exit(1);
        }
    }
}
using System;
using System.Diagnostics;

namespace xnacontentbuilder
{
    class Program
    {
        public static idContentBuilder contentBuilder = new idContentBuilder();
        private static int debugX = -1;
        private static int debugY = -1;

        public const string BASEDIR = "../rtcwcontent/";

        //
        // LockPrintPosition
        //
        public static void LockPrintPosition()
        {
            debugX = Console.CursorLeft;
            debugY = Console.CursorTop;
        }

        //
        // UnlockPrintPosition
        // 
        public static void UnlockPrintPosition()
        {
            debugX = -1;
            debugY = -1;
        }

        //
        // UpdateCursorPosition
        //
        private static void UpdateCursorPosition()
        {
            if (debugX < 0 || debugY < 0)
            {
                return;
            }

            Console.CursorLeft = debugX;
            Console.CursorTop = debugY;
        }


        //
        // Print
        //
        public static void Print(string fmt)
        {
            UpdateCursorPosition();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(fmt);
        }


        //
        // PrintColor
        //
        public static void PrintColor(string fmt, ConsoleColor color)
        {
            UpdateCursorPosition();
            Console.ForegroundColor = color;
            Console.Write(fmt);
        }

        //
        // Warning
        //
        public static void Warning(string fmt)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("WARNING: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(fmt);
        }

        //
        // Error
        //
        public static void Error(string fmt)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(fmt);
            Console.ForegroundColor = ConsoleColor.White;
            throw new Exception(fmt);
        }

       
        static void Main(string[] args)
        {
            Stopwatch operationtime = new Stopwatch();

            // Set the title for the console window.
            Console.Title = "RTCW XNA Content Builder";

            PrintColor("RTCW XNA Content Builder v0.01\n", ConsoleColor.White);
            PrintColor("(c) 2011 JV Software Inc\n", ConsoleColor.White);

            operationtime.Start();

            // Init the content builder.
            contentBuilder.Init();

            // Print the load statistics.
            contentBuilder.PrintAssetStats();

            // Finally build the content assets.
            contentBuilder.BuildContentAssets();

            // Stop the timer print the statistics and exit.
            operationtime.Stop();

            PrintColor("Operation completed in " + operationtime.Elapsed.ToString() + "\n", ConsoleColor.White);
            PrintColor("Press any key to continue...\n", ConsoleColor.White);
            Console.ReadLine();
        }
    }
}

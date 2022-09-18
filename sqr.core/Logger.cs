using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class Logger
    {
        public Level loggingLevel { get; private set; } = Level.DEBUG;

        public static Logger TEMP_STATIC_DEBUG { get; private set; }

        public enum Level
        {
            MUFFLE = 0,
            CRITICAL = 1,
            WARNINGS = 2,
            INFO = 3,
            DEBUG = 4,
            VERBOSE = 5,
            SPAM = 6
        }

        public Logger()
        {
            TEMP_STATIC_DEBUG = this;
        }

        public void logToFile(string name, string content)
        {
            if (!Directory.Exists("log")) Directory.CreateDirectory("log");
            File.WriteAllText("log\\" + name + "_" + DateTime.Now.ToFileTimeUtc(), content);
        }

        public void setLoggingLevel(Level level)
        {
            loggingLevel = level;
        }

        public void write(object message, ConsoleColor color = ConsoleColor.White, string newLineSeperator = "\n", string prefix = "")
        {
            string[] lines = message.ToString().Split(new char[] { '\n' });
            foreach (string line in lines) {
                Console.ForegroundColor = color;
                Console.Write(prefix + line + newLineSeperator);
            }
        }

        public void write(Level level, object message, ConsoleColor color = ConsoleColor.White, string newLineSeperator = "\n", string prefix = "")
        {
            if (((int)loggingLevel >= (int)level)) write(message, color, newLineSeperator, prefix);
        }

        private void writeOut(object message, ConsoleColor color = ConsoleColor.White)
        {
            Value v = null;
            if (message == null)
                message = "null";
            else {
                //if (message.GetType() == typeof(SqrError))
                //    v = (message as SqrError).data;
            }

            var _color = Console.ForegroundColor;

            string[] lines = (message + (v != null ? "\n" + v : "")).ToString().Split(new char[] { '\n' });
            foreach (string line in lines) {
                Console.ForegroundColor = color;
                Console.Write("    ~: ");
                write(line, color);
            }

            Console.ForegroundColor = _color;
        }

        public void cmd(object message, ConsoleColor color = ConsoleColor.Cyan)
        {
            writeOut(message, color);
        }

        public void error(object message, ConsoleColor color = ConsoleColor.Red)
        {
            if (((int)loggingLevel >= (int)Level.CRITICAL)) writeOut("ERROR " + message, color);
        }

        public void warn(object message, ConsoleColor color = ConsoleColor.Yellow)
        {
            if (((int)loggingLevel >= (int)Level.WARNINGS)) writeOut("WARN " + message, color);
        }

        public void info(object message, ConsoleColor color = ConsoleColor.White)
        {
            if (((int)loggingLevel >= (int)Level.INFO)) writeOut(message, color);
        }

        public void success(object message, ConsoleColor color = ConsoleColor.Green)
        {
            if (((int)loggingLevel >= (int)Level.INFO)) writeOut(message, color);
        }

        public void debug(object message, ConsoleColor color = ConsoleColor.Gray)
        {
            if (((int)loggingLevel >= (int)Level.DEBUG)) writeOut(message, color);
        }        

        public void verbose(object message, ConsoleColor color = ConsoleColor.DarkGray)
        {
            if (((int)loggingLevel >= (int)Level.VERBOSE)) writeOut(message, color);
        }

        public void spam(object message, ConsoleColor color = ConsoleColor.DarkGray)
        {
            if (((int)loggingLevel >= (int)Level.SPAM)) writeOut(message, color);
        }
    }
}

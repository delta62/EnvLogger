using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace EnvLogger
{
    public class Logger
    {
        private static readonly IDictionary<string, LogLevel> LOG_LEVELS;

        private const string  DEFAULT_TYPE = "?DEFAULT_TYPE?";

        private static readonly ConsoleColor DEFAULT_COLOR;

        private string _typeName;

        static Logger()
        {
            DEFAULT_COLOR = Console.ForegroundColor;
            LOG_LEVELS = new Dictionary<string, LogLevel>();
            ConfigureLevels();
        }

        internal static void ConfigureLevels()
        {
            LOG_LEVELS.Clear();
            LOG_LEVELS.Add(DEFAULT_TYPE, LogLevel.ERROR);

            var env = Environment.GetEnvironmentVariable("DOTNET_LOG") ?? "";
            foreach (var asm in env.Split(','))
            {
                var parts = asm.Split('=');
                if (parts.Length == 1 && !string.IsNullOrEmpty(parts[0]))
                {
                    var level = TryParseLogLevel(parts[0]);
                    LOG_LEVELS.Remove(DEFAULT_TYPE);
                    if (level == null)
                    {
                        LOG_LEVELS.Add(DEFAULT_TYPE, LogLevel.ERROR);
                    }
                    else
                    {
                        LOG_LEVELS.Add(DEFAULT_TYPE, (LogLevel)level);
                    }
                }
                else if (parts.Length == 2)
                {
                    LOG_LEVELS.Add(parts[0], TryParseLogLevel(parts[1]) ?? LogLevel.TRACE);
                }
            }
        }

        private static LogLevel? TryParseLogLevel(string input)
        {
            LogLevel res;
            if (!Enum.TryParse(input.ToUpper(), out res))
            {
                return null;
            }
            return res;
        }

        public Logger()
        {
            _typeName = new StackTrace().GetFrames()[1].GetMethod().DeclaringType.Name;
        }

        public void Trace(string fmt, params object[] args)
        {
            Log(LogLevel.TRACE, fmt, args);
        }

        public void Debug(string fmt, params object[] args)
        {
            Log(LogLevel.DEBUG, fmt, args);
        }

        public void Info(string fmt, params object[] args)
        {
            Log(LogLevel.INFO, fmt, args);
        }

        public void Warn(string fmt, params object[] args)
        {
            Log(LogLevel.WARN, fmt, args);
        }

        public void Error(string fmt, params object[] args)
        {
            Log(LogLevel.ERROR, fmt, args);
        }

        private bool ShouldLog(LogLevel level)
        {
            if (LOG_LEVELS.ContainsKey(DEFAULT_TYPE) && LOG_LEVELS[DEFAULT_TYPE] <= level)
            {
                return true;
            }
            if (LOG_LEVELS.ContainsKey(_typeName) && LOG_LEVELS[_typeName] <= level)
            {
                return true;
            }
            return false;
        }

        private static ConsoleColor GetConsoleColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.DEBUG:
                return ConsoleColor.Blue;
                case LogLevel.INFO:
                return ConsoleColor.Green;
                case LogLevel.WARN:
                return ConsoleColor.Yellow;
                case LogLevel.ERROR:
                return ConsoleColor.Red;
                default:
                return DEFAULT_COLOR;
            }
        }

        private void Log(LogLevel level, string fmt, params object[] args)
        {
            if (!ShouldLog(level))
            {
                return;
            }

            Console.ForegroundColor = GetConsoleColor(level);
            var ts = DateTime.Now.ToString("yyyy-MM-ddTHH:MM:ssZ");
            Console.Error.Write($"{level}: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Error.Write($"{ts}: {_typeName}: ");
            Console.ForegroundColor = DEFAULT_COLOR;
            Console.Error.WriteLine($"{fmt}", args);
        }
    }
}

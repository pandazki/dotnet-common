using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandazki.Common.Log
{
    public static class Log
    {
        private static readonly ILogger VerboseLogger;
        private static readonly ILogger InformationLogger;
        private static readonly ILogger WarningLogger;
        private static readonly ILogger ErrorLogger;
        private static readonly ILogger CriticalLogger;

        static Log()
        {
            VerboseLogger = LoggerFactory.GetLogger("Verbose");
            InformationLogger = LoggerFactory.GetLogger("Information");
            WarningLogger = LoggerFactory.GetLogger("Warning");
            ErrorLogger = LoggerFactory.GetLogger("Error");
            CriticalLogger = LoggerFactory.GetLogger("Critical");
        }

        public static void Verbose(string message)
        {
            VerboseLogger.Verbose(message);
        }

        public static void Information(string message)
        {
            InformationLogger.Information(message);
        }

        public static void Information(string format, params object[] args)
        {
            InformationLogger.Information(string.Format(format, args));
        }

        public static void Error(string format, params object[] args)
        {
            ErrorLogger.Error(string.Format(format, args));
        }

        public static void Warning(string message)
        {
            WarningLogger.Warning(message);
        }

        public static void Error(string message)
        {
            ErrorLogger.Error(message);
        }

        public static void Critical(string message)
        {
            CriticalLogger.Critical(message);
        }

        public static void Verbose(object message)
        {
            VerboseLogger.Verbose(message.ToString());
        }

        public static void Information(object message)
        {
            InformationLogger.Information(message.ToString());
        }

        public static void Warning(object message)
        {
            WarningLogger.Warning(message.ToString());
        }

        public static void Error(object message)
        {
            ErrorLogger.Error(message.ToString());
        }

        public static void Critical(object message)
        {
            CriticalLogger.Critical(message.ToString());
        }
    }
}

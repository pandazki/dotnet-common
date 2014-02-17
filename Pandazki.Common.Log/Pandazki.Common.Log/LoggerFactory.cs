using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandazki.Common.Log
{
    public static class LoggerFactory
    {
        private static readonly Dictionary<string, Logger> Dict = new Dictionary<string, Logger>();

        public static ILogger GetLogger(string sourcename)
        {
            lock (Dict)
            {
                Logger logger;
                Dict.TryGetValue(sourcename, out logger);
                if (logger == null)
                {
                    logger = new Logger(new TraceSource(sourcename));
                    Dict[sourcename] = logger;
                }
                return logger;
            }
        }
    }
}

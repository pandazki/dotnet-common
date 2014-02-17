using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandazki.Common.Log
{
    /// <summary>
    /// Simple logger based on System.Diagnostics.TraceSource
    /// </summary>
    public class Logger : ILogger
    {
        private const string DefaultSourceName = "Default";
        private readonly TraceSource _ts;
        public string SourceName
        {
            get { return _ts.Name; }
        }

        internal Logger(TraceSource ts)
        {
            _ts = ts.Listeners.Count > 0 ? ts : new TraceSource(DefaultSourceName);
        }

        #region ILogger Member

        public void Critical(string msg)
        {
            _ts.TraceEvent(TraceEventType.Critical, 0, msg);
        }

        public void Error(string msg)
        {
            _ts.TraceEvent(TraceEventType.Error, 0, msg);
        }

        public void Warning(string msg)
        {
            _ts.TraceEvent(TraceEventType.Warning, 0, msg);
        }

        public void Information(string msg)
        {
            _ts.TraceEvent(TraceEventType.Information, 0, msg);
        }

        public void Verbose(string msg)
        {
            _ts.TraceEvent(TraceEventType.Verbose, 0, msg);
        }

        #endregion
    }
}

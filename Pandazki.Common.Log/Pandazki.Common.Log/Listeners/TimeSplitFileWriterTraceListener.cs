using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandazki.Common.Log.Utils;

namespace Pandazki.Common.Log.Listeners
{
    /// <summary>
    /// a general log to file listener.
    /// support auto split log file by time.
    /// </summary>
    public class TimeSplitFileWriterTraceListener : TraceListener
    {
        private const string DefaultFileName = "Log_{0:yyyyMMddHH}.txt";
        private readonly FileTextWriter _logWriter;

        public TimeSplitFileWriterTraceListener()
            : this(DefaultFileName)
        {

        }
        public TimeSplitFileWriterTraceListener(string filename)
            : base(filename)
        {
            _logWriter = FileTextWriter.CreateSyncLogWriter(
                (x, y) => (x.Date == y.Date && x.Hour == y.Hour) ? 0 : 1,
                (x) => string.Format(filename, x)
            );
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            WriteLine(eventCache.DateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.ffff") + " ThreadId:" + eventCache.ThreadId + " " + source + "\r\n" + message + "\r\n");
        }

        public override void Write(string message)
        {
            _logWriter.Write(message);
        }

        public override void WriteLine(string message)
        {
            _logWriter.WriteLine(message);
        }

        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Pandazki.Common.Log.Utils
{
    /// <summary>
    /// the file text writer from @yzh's LogWriter
    /// </summary>
    public sealed class FileTextWriter
        : TextWriter
    {

        #region Fields
        private const int MaxCapacity = 128;
        /// <summary>
        /// the comparison of date time
        /// </summary>
        private readonly Comparison<DateTime> dateDiffer;
        /// <summary>
        /// get the file's name of date time
        /// </summary>
        private readonly Func<DateTime, string> fileNameGetter;
        /// <summary>
        /// the inner writter
        /// </summary>
        private StreamWriter m_writer;
        /// <summary>
        /// current inner writter's date time
        /// </summary>
        private DateTime dt;
        private Queue<string> m_queue = new Queue<string>(MaxCapacity);
        private volatile bool m_disposed = false;
        private readonly Thread m_guarder;
        private readonly object SyncRoot = new object();
        #endregion

        #region Ctors

        /// <summary>
        /// ctor
        /// </summary>
        private FileTextWriter(Comparison<DateTime> dateDiffer,
            Func<DateTime, string> fileNameGetter)
        {
            if (dateDiffer == null)
                throw new ArgumentNullException("dateDiffer");
            if (fileNameGetter == null)
                throw new ArgumentNullException("fileNameGetter");
            this.dateDiffer = dateDiffer;
            this.fileNameGetter = fileNameGetter;
            m_guarder = new Thread(GuardWriteToFile);
            m_guarder.IsBackground = true;
            m_guarder.Start();
        }

        #endregion

        #region Factory Method

        /// <summary>
        /// create a thread safe log writer
        /// </summary>
        /// <returns>a thread safe log writer</returns>
        public static FileTextWriter CreateSyncLogWriter(
            Comparison<DateTime> dateDiffer,
            Func<DateTime, string> fileNameGetter)
        {
            if (dateDiffer == null)
                throw new ArgumentNullException("dateDiffer");
            if (fileNameGetter == null)
                throw new ArgumentNullException("fileNameGetter");
            return new FileTextWriter(dateDiffer, fileNameGetter);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// create inner writer
        /// </summary>
        private StreamWriter CreateWriter(string fileName)
        {
            DirectoryInfo di = (new FileInfo(fileName)).Directory;
            if (!di.Exists)
                di.Create();
            return new StreamWriter(new FileStream(fileName,
                FileMode.Append, FileAccess.Write, FileShare.ReadWrite,
                0x4000, FileOptions.SequentialScan), Encoding, 0x1000);
        }

        /// <summary>
        /// the inner writer
        /// </summary>
        private StreamWriter GetWriter()
        {
            DateTime now = DateTime.Now;
            if (m_writer == null)
            {
                m_writer = CreateWriter(fileNameGetter(now));
                dt = now;
                return m_writer;
            }
            if (dateDiffer(dt, now) == 0)
            {
                return m_writer;
            }
            else
            {
                m_writer.Dispose();
                m_writer = CreateWriter(fileNameGetter(now));
                dt = now;
                return m_writer;
            }
        }

        /// <summary>
        /// Ensure file created.
        /// </summary>
        internal void EnsureFileCreated()
        {
            if (m_writer == null)
                lock (SyncRoot)
                    GetWriter();
            else if (dateDiffer(dt, DateTime.Now) != 0)
                lock (SyncRoot)
                    GetWriter();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Overrided.
        /// </summary>
        public override void Write(char value)
        {
            EnqueueItem(value.ToString());
        }

        /// <summary>
        /// Overrided.
        /// </summary>
        public override void Write(char[] buffer)
        {
            EnqueueItem(new string(buffer));
        }

        /// <summary>
        /// Overrided.
        /// </summary>
        public override void Write(char[] buffer, int index, int count)
        {
            EnqueueItem(new string(buffer, index, count));
        }

        /// <summary>
        /// Overrided.
        /// </summary>
        public override void Write(string value)
        {
            EnqueueItem(value);
        }

        /// <summary>
        /// Overrided.
        /// </summary>
        public override void WriteLine(object value)
        {
            var enumerable = value as IEnumerable<string>;
            if (enumerable != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in enumerable)
                    sb.AppendLine(item);
                EnqueueItem(sb.ToString());
            }
            else
                base.WriteLine(value);
        }

        /// <summary>
        /// Overrided.
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// Overrided.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            m_disposed = true;
            m_guarder.Join();
            WriteToFile();
            if (disposing)
            {
                lock (SyncRoot)
                    if (m_writer != null)
                    {
                        m_writer.Dispose();
                        m_writer = null;
                    }
            }
            base.Dispose(disposing);
        }

        ~FileTextWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// encoding is gbk
        /// </summary>
        public override Encoding Encoding
        {
            get { return Encoding.GetEncoding("GBK"); }
        }

        #endregion

        #region Private Implements

        private string DequeueItem()
        {
            lock (m_queue)
                if (m_queue.Count > 0)
                {
                    Monitor.PulseAll(m_queue);
                    return m_queue.Dequeue();
                }
            return null;
        }

        private void EnqueueItem(string value)
        {
            EnsureNotDisposed();
            lock (m_queue)
            {
                while (m_queue.Count == MaxCapacity)
                {
                    Monitor.Wait(m_queue, 100);
                }
                m_queue.Enqueue(value);
                Monitor.PulseAll(m_queue);
            }
        }

        private void GuardWriteToFile()
        {
            while (true)
            {
                bool hasItem;
                lock (m_queue)
                {
                    hasItem = m_queue.Count > 0;
                    while (!hasItem)
                    {
                        if (m_disposed)
                            return;
                        if (Monitor.Wait(m_queue, 100))
                            break;
                        hasItem = m_queue.Count > 0;
                    }
                }
                if (hasItem)
                {
                    m_guarder.IsBackground = false;
                    try
                    {
                        WriteToFile();
                    }
                    finally
                    {
                        m_guarder.IsBackground = true;
                    }
                }
            }
        }

        private void WriteToFile()
        {
            lock (SyncRoot)
            {
                string s;
                StreamWriter writer = null;
                while ((s = DequeueItem()) != null)
                {
                    if (writer == null)
                        writer = GetWriter();
                    try
                    {
                        writer.Write(s);
                    }
                    catch (ObjectDisposedException)
                    {
                        m_writer = null;
                        writer = GetWriter();
                        writer.Write(s);
                    }
                }
                if (writer != null)
                {
                    try
                    {
                        writer.Flush();
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        private void EnsureNotDisposed()
        {
            if (m_disposed)
                throw new ObjectDisposedException("FileTextWriter");
        }

        #endregion

    }
}

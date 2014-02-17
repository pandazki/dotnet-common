using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandazki.Common.Log
{
    public interface ILogger
    {
        void Critical(string msg);
        void Error(string msg);
        void Warning(string msg);
        void Information(string msg);
        void Verbose(string msg);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandazki.Common.Log.Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            //see the app.config -> system.diagnostics 
            //simple usage
            Log.Critical("Critical");
            Log.Information("Infomation");

            //custom source
            var log = LoggerFactory.GetLogger("CustomInfoSource");
            log.Information("Custom Info");

            Console.ReadKey();
        }
    }
}

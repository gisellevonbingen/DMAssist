using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssist
{
    public class CrashReport
    {
        public string DumpFile { get; }
        public Exception Exception { get; }

        public CrashReport(string dumpFile, Exception exception)
        {
            this.DumpFile = dumpFile;
            this.Exception = exception;
        }

    }

}

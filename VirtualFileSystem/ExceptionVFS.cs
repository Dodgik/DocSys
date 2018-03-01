using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace IO.VFS
{
    [Serializable()]
    public class ExceptionVFS : System.Exception
    {
        public ExceptionVFS(): base(){}
        //
        public ExceptionVFS(string message) : base(message) {}
        //
        public ExceptionVFS(string message, Exception innerException) : base(message, innerException) { }
    }
}

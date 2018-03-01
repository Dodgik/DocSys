using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace IO.VFS
{
    [Serializable()]
    public class CustomException : System.Exception
    {
        public CustomException(): base(){}
        //
        public CustomException(string message) : base(message) {}
        //
        public CustomException(string message, Exception innerException) : base(message, innerException) { }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace PermissionMembership
{
    [Serializable()]
    public class PermissionMembershipException : System.Exception
    {
        public PermissionMembershipException() : base() { }
        //
        public PermissionMembershipException(string message) : base(message) { }
        //
        public PermissionMembershipException(string message, Exception innerException) : base(message, innerException) { }
    }
}

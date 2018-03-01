using System;

namespace BizObj.CustomException
{
    [Serializable]
    public class DocumentException : CustomException
    {
        public DocumentException() { }
        
        public DocumentException(string message) : base(message) {}
        
        public DocumentException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class AccessException : DocumentException
    {
        private const string AccessDeniedMessage = "User Name: \"{0}\". Action: \"{1}\". Access Denied";

        public AccessException() { }
        
        public AccessException(string message) : base(message) { }
        
        public AccessException(string message, Exception innerException) : base(message, innerException) { }

        public AccessException(string userName, string action): this(String.Format(AccessDeniedMessage, userName, action))
        {

        }
    }

    [Serializable]
    public class NotFoundException : CustomException
    {
        private const string NotFoundMessage = "The \"{0}\" \"{1}\" was not found.";

        public NotFoundException() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }

        public NotFoundException(string dataType, string dataValue) : this(String.Format(NotFoundMessage, dataType, dataValue))
        {

        }
    }
}
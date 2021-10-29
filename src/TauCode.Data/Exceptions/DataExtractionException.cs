using System;
using System.Runtime.Serialization;

namespace TauCode.Data.Exceptions
{
    [Serializable]
    public class DataExtractionException : DataException
    {
        public DataExtractionException()
        {
        }

        public DataExtractionException(string message)
            : base(message)
        {
        }

        public DataExtractionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected DataExtractionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}

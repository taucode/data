using System;
using System.Runtime.Serialization;

namespace TauCode.Data.Exceptions
{
    [Serializable]
    public class TextDataExtractionException : Exception
    {
        public TextDataExtractionException()
            : base("Text data extraction failed.")
        {
        }

        public TextDataExtractionException(string message)
            : base(message)
        {
        }

        public TextDataExtractionException(string message, int? errorIndex)
            : base(message)
        {
            this.ErrorIndex = errorIndex;
        }

        public TextDataExtractionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public TextDataExtractionException(string message, int? errorIndex, Exception inner)
            : base(message, inner)
        {
            this.ErrorIndex = errorIndex;
        }

        protected TextDataExtractionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public int? ErrorIndex { get; }

        internal ExtractionError? ExtractionError;
    }
}

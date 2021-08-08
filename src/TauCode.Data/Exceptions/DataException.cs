﻿using System;
using System.Runtime.Serialization;

namespace TauCode.Data.Exceptions
{
    [Serializable]
    public class DataException : Exception
    {
        public DataException()
        {
        }

        public DataException(string message)
            : base(message)
        {
        }

        public DataException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected DataException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}

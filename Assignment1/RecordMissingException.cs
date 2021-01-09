using System;
using System.Runtime.Serialization;

namespace Assignment1
{
    internal class RecordMissingException : Exception
    {
        public RecordMissingException()
        {
        }

        public RecordMissingException(string message) : base(message)
        {
        }

        public RecordMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecordMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
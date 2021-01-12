using System;
using System.Runtime.Serialization;

namespace Assignment1
{
    public class InputCancelException : Exception
    {
        public InputCancelException() : base("The user cancelled their input and the cancellation wasn't caught")
        {
        }

        public InputCancelException(string message) : base(message)
        {
        }

        public InputCancelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InputCancelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
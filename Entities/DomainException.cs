using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Entities
{
    public class ÀpplicationException : Exception
    {
        public ÀpplicationException()
        {
        }

        public ÀpplicationException(string? message) : base(message)
        {
        }

        public ÀpplicationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}

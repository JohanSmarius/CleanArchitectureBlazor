using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Application
{
    public class ApplicationLayerException : Exception
    {
        public ApplicationLayerException()
        {
        }

        public ApplicationLayerException(string? message) : base(message)
        {
        }

        public ApplicationLayerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        
    }
}

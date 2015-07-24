using System;

namespace SDDB
{
    [Serializable()]
    public class DbBadRequestException : System.Exception
    {
        public DbBadRequestException() : base() { }
        public DbBadRequestException(string message) : base(message) { }
        public DbBadRequestException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an 
        // exception propagates from a remoting server to the client.  
        protected DbBadRequestException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}


namespace MyGenericContext.Exceptions
{
    public class DatabaseContextException : System.Exception
    {
        public DatabaseContextException() : base() { }
        public DatabaseContextException(string message) : base(message) { }
        public DatabaseContextException(string message, System.Exception inner) : base(message, inner) { }

        // // A constructor is needed for serialization when an
        // // exception propagates from a remoting server to the client. 
        // protected DatabaseContextException(System.Runtime.Serialization.SerializationInfo info,
        //     System.Runtime.Serialization.StreamingContext context) { }
        // }
    }
}
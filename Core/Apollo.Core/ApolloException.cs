using System;
using System.Runtime.Serialization;

namespace Apollo.Core
{
    [Serializable]
    public class ApolloException : Exception
    {
        public ApolloException()
        {
        }

        public ApolloException(string message)
            : base(message)
        {
        }

        public ApolloException(string messageFormat, params object[] args)
			: base(string.Format(messageFormat, args))
		{
        }

        protected ApolloException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ApolloException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

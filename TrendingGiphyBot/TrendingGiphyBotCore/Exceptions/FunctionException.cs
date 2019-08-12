using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace TrendingGiphyBotCore.Exceptions
{
    public class FunctionException : Exception
    {
        public FunctionException() { }
        public FunctionException(string message) : base(message) { }
        public FunctionException(string message, Exception innerException) : base(message, innerException) { }
        protected FunctionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public FunctionException(HttpRequestMessage request, HttpResponseMessage response) :
            this($"Error: {request.Method.Method} {request.RequestUri.AbsoluteUri} {response.StatusCode.ToString()} {response.ReasonPhrase}")
        { }
    }
}

﻿using System;
using System.Runtime.Serialization;

namespace TrendingGiphyBot.Exceptions
{
    class FunctionHelperException : Exception
    {
        public FunctionHelperException()
        {
        }

        public FunctionHelperException(string message) : base(message)
        {
        }

        public FunctionHelperException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FunctionHelperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

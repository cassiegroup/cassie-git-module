using System;

namespace cassie.git.module
{
    public class LenghNotMatchException : Exception
    {
        public LenghNotMatchException(){}

        public LenghNotMatchException(string message) : base(message){}

        public LenghNotMatchException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
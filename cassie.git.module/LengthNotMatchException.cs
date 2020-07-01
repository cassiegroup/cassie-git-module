using System;

namespace cassie.git.module
{
    public class LengthNotMatchException : Exception
    {
        public LengthNotMatchException(){}

        public LengthNotMatchException(string message) : base(message){}

        public LengthNotMatchException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
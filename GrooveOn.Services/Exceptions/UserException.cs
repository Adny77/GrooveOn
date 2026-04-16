using System;

namespace GrooveOn.Services.Exceptions
{
    public class UserException : Exception
    {
        public UserException() : base()
        {
        }

        public UserException(string message) : base(message)
        {
        }
    }
}
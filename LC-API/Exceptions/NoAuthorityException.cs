using System;

namespace LC_API.Exceptions
{
    internal class NoAuthorityException : Exception
    {
        internal NoAuthorityException(string message) : base(message) { }
    }
}

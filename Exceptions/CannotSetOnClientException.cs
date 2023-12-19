using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.Exceptions
{
    internal class CannotSetOnClientException : Exception
    {
        internal CannotSetOnClientException(string message) : base(message) {}
    }
}

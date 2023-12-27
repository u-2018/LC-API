using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.Exceptions
{
    internal class NoAuthorityException : Exception
    {
        internal NoAuthorityException(string message) : base(message) {}
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia.Core.Exceptions
{
    /// <summary>
    /// When the number of Uses has reached is max
    /// </summary>
    class MaxUsesException : Exception
    {
        public MaxUsesException(string message) : base(message)
        {

        }
    }
}

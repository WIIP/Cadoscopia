using System;
using System.ComponentModel;

namespace Cadoscopia.Core
{
    public class UserException : Exception
    {
        public UserException([Localizable(true)] string message) : base(message)
        {
        }
    }
}
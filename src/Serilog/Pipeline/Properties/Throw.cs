using System;

namespace Serilog.Pipeline.Properties
{
    static class Throw
    {
        public static void NewArgumentOutOfRangeException(string paramName, string message)
        {
            throw new ArgumentOutOfRangeException(paramName, message);
        }

        public static void NewArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
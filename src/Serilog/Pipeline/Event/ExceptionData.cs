using System;

namespace Serilog.Pipeline.Event
{
    // Could carry Message, StackTrace, etc., but there's no complete mapping that will fit all
    // exception types
    struct ExceptionData
    {
        readonly string _asString;

        public static readonly ExceptionData None = default;

        public ExceptionData(string asString)
        {
            _asString = asString ?? throw new ArgumentNullException(nameof(asString));
        }

        public override string ToString()
        {
            return _asString ?? "";
        }
    }
}

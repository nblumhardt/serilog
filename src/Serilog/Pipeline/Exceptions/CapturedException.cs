using System;
using System.Collections;

namespace Serilog.Pipeline.Exceptions
{
    // An exception that, while still unavoidably mutable in some respects, is not affected by
    // rethrowing of the source Exception instance. This is intended to be used in situations
    // where a log event carrying an exception is transferred between threads.
    // This might look more like AggregateException in the future, carrying separate message/
    // stack trace/data etc. per node. Unfortunately all representations, including the
    // current one, require some amount of allocation and eager evaluation of lazy members on
    // the source exception.
    class CapturedException : Exception
    {
        readonly string _toString;

        public CapturedException(Exception source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            _toString = source.ToString();

            foreach (DictionaryEntry data in source.Data)
                Data.Add(data.Key, data.Value);
        }

        public override string ToString()
        {
            return _toString;
        }
    }
}

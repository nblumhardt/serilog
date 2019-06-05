using System;
using Serilog.Core;
using Serilog.Events;
using Serilog.Pipeline.Adapters;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Write
{
    // Base class for user-defined sinks.
    abstract class Sink : Emitter, ILogEventSink
    {
        // An inefficient default implementation that ensures all P2 sinks can still be used in code
        // that expects an `ILogEventSink`. Heavily-used enrichers will override this method and implement
        // it efficiently.
        public virtual void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            var data = Adapt.ToEventData(logEvent);
            Emit(in data);
        }
    }
}

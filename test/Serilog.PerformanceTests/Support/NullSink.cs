using Serilog.Events;
using Serilog.Pipeline.Event;
using Serilog.Pipeline.Write;

namespace Serilog.PerformanceTests.Support
{
    class NullSink : Sink
    {
        public override void Emit(LogEvent logEvent)
        {
        }

        public override void Emit(in EventData data)
        {
        }
    }
}

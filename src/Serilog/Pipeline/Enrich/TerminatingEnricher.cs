using Serilog.Core;
using Serilog.Events;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Enrich
{
    class TerminatingEnricher : Enricher
    {
        public static readonly TerminatingEnricher Instance = new TerminatingEnricher();

        public override void Enrich(ref EventDataBuilder eventDataBuilder, ILogEventPropertyValueFactory propertyValueFactory)
        {
        }

        public override int EstimatedCount { get; } = 0;

        public override void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
        }
    }
}

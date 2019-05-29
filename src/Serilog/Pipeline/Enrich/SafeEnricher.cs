using System;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Enrich
{
    class SafeEnricher<TEnricher> : Element<EventData>
        where TEnricher : Enricher
    {
        readonly ILogEventPropertyValueFactory _propertyValueFactory;
        readonly TEnricher _enricher;

        public SafeEnricher(TEnricher enricher, ILogEventPropertyValueFactory propertyValueFactory)
        {
            _propertyValueFactory = propertyValueFactory;
            _enricher = enricher;
        }

        public override void Propagate(in EventData eventData, Emitter<EventData> next)
        {
            var builder = eventData.ToBuilder(1);

            try
            {
                _enricher.Enrich(ref builder, _propertyValueFactory);
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Exception {0} caught while enriching {1} with {2}.", ex, eventData, _enricher);
            }

            var enriched = builder.ToImmutable();
            next.Emit(in enriched);
        }
    }
}

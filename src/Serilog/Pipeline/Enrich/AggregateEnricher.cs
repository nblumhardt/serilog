using System;
using System.Linq;
using Serilog.Core;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Enrich
{
    sealed class AggregateEnricher : Element<EventData>
    {
        readonly ILogEventPropertyValueFactory _propertyValueFactory;
        readonly Enricher[] _enrichers;

        public AggregateEnricher(Enricher[] enrichers, ILogEventPropertyValueFactory propertyValueFactory)
        {
            _propertyValueFactory = propertyValueFactory;
            _enrichers = enrichers?.ToArray() ?? throw new ArgumentNullException(nameof(enrichers));
        }

        public override void Propagate(in EventData eventData, Emitter<EventData> next)
        {
            // Assume each enricher will, on average, add a property; plus two for good measure...

            var builder = eventData.ToBuilder(_enrichers.Length + 2);
            foreach (var enricher in _enrichers)
            {
                enricher.Enrich(ref builder, _propertyValueFactory);
            }

            var enriched = builder.ToImmutable();
            next.Emit(in enriched);
        }
    }
}

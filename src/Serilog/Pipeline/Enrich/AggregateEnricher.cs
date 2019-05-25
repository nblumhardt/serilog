using System;
using System.Linq;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Enrich
{
    sealed class AggregateEnricher<TException> : Element<EventData<TException>>
    {
        readonly Enricher<TException>[] _enrichers;

        public AggregateEnricher(params Enricher<TException>[] enrichers)
        {
            _enrichers = enrichers?.ToArray() ?? throw new ArgumentNullException(nameof(enrichers));
        }

        public override void Propagate(in EventData<TException> eventData, Emitter<EventData<TException>> next)
        {
            // Assume each enricher will, on average, add a property; plus two for good measure...

            var builder = EventDataBuilder<TException>.FromEventData(in eventData, _enrichers.Length + 2);
            foreach (var enricher in _enrichers)
            {
                enricher.Enrich(ref builder);
            }

            var enriched = builder.ToEventData();
            next.Emit(in enriched);
        }
    }
}

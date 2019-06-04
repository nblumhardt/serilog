using System;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Enrich
{
    class ChainedEnricher<TEnricher> : Enricher
        where TEnricher : Enricher
    {
        readonly TEnricher _enricher;
        readonly Enricher _tail;

        public ChainedEnricher(TEnricher enricher, Enricher tail)
        {
            _enricher = enricher;
            _tail = tail;
            EstimatedCount = _enricher.EstimatedCount + _tail.EstimatedCount;
        }

        public override void Enrich(ref EventDataBuilder eventDataBuilder, ILogEventPropertyValueFactory propertyValueFactory)
        {
            try
            {
                _enricher.Enrich(ref eventDataBuilder, propertyValueFactory);
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Exception {0} caught while enriching {1} with {2}.", ex, eventDataBuilder, _enricher);
            }

            _tail.Enrich(ref eventDataBuilder, propertyValueFactory);
        }

        public override int EstimatedCount { get; }
    }
}

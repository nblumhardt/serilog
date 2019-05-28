using Serilog.Core;
using Serilog.Events;
using Serilog.Pipeline.Adapters;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Enrich
{
    // Base class for user-defined enrichers.
    abstract class Enricher : ILogEventEnricher
    {
        public abstract void Enrich(ref EventDataBuilder eventDataBuilder, ILogEventPropertyValueFactory propertyFactory);

        // An inefficient default implementation that ensures all P2 enrichers can still be used in code
        // that expects an `ILogEventEnricher`. Heavily-used enrichers will override this method and implement
        // it efficiently.
        public virtual void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var builder = Adapt.ToEventDataBuilder(logEvent, 1);
            var propertyValueFactory = Adapt.ToPropertyValueFactory(propertyFactory);

            Enrich(ref builder, propertyValueFactory);

            Adapt.UpdateProperties(builder.Properties, logEvent);
        }
    }
}

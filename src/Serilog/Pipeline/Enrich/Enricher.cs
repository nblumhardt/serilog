using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;
using Serilog.Pipeline.Event;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Enrich
{
    abstract class Enricher<TException> : ILogEventEnricher
    {
        public abstract void Enrich(ref EventDataBuilder<TException> eventDataBuilder);

        // An inefficient default implementation that ensures all P2 enrichers can still be used in code
        // that expects an `ILogEventEnricher`. Heavily-used enrichers should override this method and implement
        // it efficiently.
        public virtual void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            // Only Enricher<Exception> subclasses can interact with the exception in classic pipelines by default; to work
            // around this restriction, this method can be overridden.
            TException exception = default;
            if (logEvent.Exception is TException ex)
                exception = ex;

            var properties = new EventPropertiesBuilder(logEvent.Properties.Select(kvp => (kvp.Key, kvp.Value)).ToArray(), 0);

            var builder = new EventDataBuilder<TException>(logEvent.Timestamp, logEvent.Level, exception, logEvent.MessageTemplate, properties);
            Enrich(ref builder);

            var names = logEvent.Properties.Keys.ToArray();
            foreach (var name in names)
                logEvent.RemovePropertyIfPresent(name);

            foreach (var (name, value) in builder.Properties)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty(name, value));
            }
        }
    }
}

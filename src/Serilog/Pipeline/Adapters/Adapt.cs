﻿using System;
using System.Linq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Pipeline.Event;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Adapters
{
    static class Adapt
    {
        public static EventDataBuilder ToEventDataBuilder(LogEvent logEvent, int propertiesReservedCapacity)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            var properties = new EventPropertiesBuilder(logEvent.Properties.Count + propertiesReservedCapacity);
            foreach (var logEventProperty in logEvent.Properties)
                properties.AddUnchecked(new EventProperty(logEventProperty.Key, logEventProperty.Value));
            return new EventDataBuilder(logEvent.Timestamp, logEvent.Level, logEvent.Exception, logEvent.MessageTemplate, properties);
        }

        public static EventData ToEventData(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            return ToEventDataBuilder(logEvent, 0).ToImmutable();
        }

        public static LogEventPropertyValueFactory ToPropertyValueFactory(ILogEventPropertyFactory propertyFactory)
        {
            if (propertyFactory == null) throw new ArgumentNullException(nameof(propertyFactory));
            return new LogEventPropertyValueFactory(propertyFactory);
        }

        public static void UpdateProperties(EventPropertiesBuilder source, LogEvent dest)
        {
            if (dest == null) throw new ArgumentNullException(nameof(dest));

            var names = dest.Properties.Keys.ToArray();
            foreach (var name in names)
                dest.RemovePropertyIfPresent(name);

            foreach (var (name, value) in source)
            {
                dest.AddOrUpdateProperty(new LogEventProperty(name, value));
            }
        }
    }
}

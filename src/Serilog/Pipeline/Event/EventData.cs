using System;
using Serilog.Events;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Event
{
    readonly struct EventData
    {
#pragma warning disable 414
        public static readonly EventData Empty = default;
#pragma warning restore 414

        public DateTimeOffset Timestamp { get; }
        public LogEventLevel Level { get; }
        public MessageTemplate MessageTemplate { get; }
        public EventProperties Properties { get; }
        public Exception Exception { get; }

        public EventData(DateTimeOffset timestamp, LogEventLevel level, Exception exception, MessageTemplate messageTemplate, EventProperties properties)
        {
            Timestamp = timestamp;
            Level = level;
            MessageTemplate = messageTemplate ?? throw new ArgumentNullException(nameof(messageTemplate));
            Properties = properties;
            Exception = exception;
        }

        public EventDataBuilder ToBuilder(int propertiesReservedCapacity)
        {
            return new EventDataBuilder(
                Timestamp,
                Level,
                Exception,
                MessageTemplate,
                Properties.ToBuilder(propertiesReservedCapacity));
        }
    }
}

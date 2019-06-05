using System;
using Serilog.Events;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Event
{
    readonly ref struct EventData
    {
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

        public CapturedEventData Capture()
        {
            return new CapturedEventData(in this);
        }
    }
}

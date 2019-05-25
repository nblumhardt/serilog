using System;
using Serilog.Events;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Event
{
    struct EventData<TException>
    {
        public static readonly EventData<TException> Empty = default;

        public DateTimeOffset Timestamp { get; }
        public LogEventLevel Level { get; }
        public MessageTemplate MessageTemplate { get; }
        public EventProperties Properties { get; }
        public TException Exception { get; }

        public EventData(DateTimeOffset timestamp, LogEventLevel level, TException exception, MessageTemplate messageTemplate, EventProperties properties)
        {
            Timestamp = timestamp;
            Level = level;
            MessageTemplate = messageTemplate;
            Properties = properties;
            Exception = exception;
        }
    }
}

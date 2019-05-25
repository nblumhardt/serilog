using System;
using Serilog.Events;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Event
{
    struct EventDataBuilder
    {
        MessageTemplate _messageTemplate;

        public DateTimeOffset Timestamp { get; set; }
        public LogEventLevel Level { get; set; }
        public Exception Exception { get; set; }
        public EventPropertiesBuilder Properties { get; set; }

        public MessageTemplate MessageTemplate
        {
            get => _messageTemplate;
            set => _messageTemplate = value ?? throw new ArgumentNullException(nameof(value));
        }

        public EventDataBuilder(DateTimeOffset timestamp, LogEventLevel level, Exception exception, MessageTemplate messageTemplate, EventPropertiesBuilder properties)
        {
            _messageTemplate = messageTemplate ?? throw new ArgumentNullException(nameof(messageTemplate));

            Timestamp = timestamp;
            Level = level;
            Properties = properties;
            Exception = exception;
        }

        public EventData ToImmutable()
        {
            return new EventData(Timestamp, Level, Exception, MessageTemplate, Properties.ToImmutable());
        }
    }
}

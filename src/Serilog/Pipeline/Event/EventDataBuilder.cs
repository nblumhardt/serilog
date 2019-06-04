using System;
using Serilog.Events;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Event
{
    struct EventDataBuilder
    {
        MessageTemplate _messageTemplate;
        EventPropertiesBuilder _properties;

        public DateTimeOffset Timestamp { get; set; }
        public LogEventLevel Level { get; set; }
        public Exception Exception { get; set; }

        public EventPropertiesBuilder Properties
        {
            get => _properties;
            set => _properties = value;
        }

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
            _properties = properties;
            Exception = exception;
        }

        public EventData ToImmutable()
        {
            return new EventData(Timestamp, Level, Exception, MessageTemplate, Properties.ToImmutable());
        }

        // This method is "dangerous" in that storage backing Properties transfers in ownership without copying.
        public static EventData IntoImmutable(ref EventDataBuilder builder)
        {
            return new EventData(builder.Timestamp, builder.Level, builder.Exception, builder.MessageTemplate, EventPropertiesBuilder.IntoImmutable(ref builder._properties));
        }
    }
}

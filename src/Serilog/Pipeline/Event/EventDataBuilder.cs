using System;
using Serilog.Events;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Event
{
    struct EventDataBuilder<TException>
    {
        MessageTemplate _messageTemplate;

        public DateTimeOffset Timestamp { get; set; }
        public LogEventLevel Level { get; set; }
        public TException Exception { get; set; }
        public EventPropertiesBuilder Properties { get; set; }

        public MessageTemplate MessageTemplate
        {
            get => _messageTemplate;
            set => _messageTemplate = value ?? throw new ArgumentNullException(nameof(value));
        }

        public EventDataBuilder(DateTimeOffset timestamp, LogEventLevel level, TException exception, MessageTemplate messageTemplate, EventPropertiesBuilder properties)
        {
            _messageTemplate = messageTemplate ?? throw new ArgumentNullException(nameof(messageTemplate));

            Timestamp = timestamp;
            Level = level;
            Properties = properties;
            Exception = exception;
        }

        public static EventDataBuilder<TException> FromEventData(in EventData<TException> eventData, int propertiesReservedCapacity)
        {
            return new EventDataBuilder<TException>(
                eventData.Timestamp,
                eventData.Level,
                eventData.Exception,
                eventData.MessageTemplate,
                eventData.Properties.ToBuilder(propertiesReservedCapacity));
        }

        public EventData<TException> ToEventData()
        {
            return new EventData<TException>(Timestamp, Level, Exception, MessageTemplate, Properties.ToImmutable());
        }
    }
}

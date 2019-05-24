//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Serilog.Events;
//using Serilog.Parsing;

//namespace Serilog.Pipeline
//{

//    struct EventData
//    {
//        public static readonly EventData Empty = default;

//        public DateTimeOffset Timestamp { get; }
//        public LogEventLevel Level { get; }
//        public MessageTemplate MessageTemplate { get; }
//        public ImmutableDictionary<string, LogEventPropertyValue> Properties { get; }
//        public Exception Exception { get; }

//        public EventData(DateTimeOffset timestamp, LogEventLevel level, Exception exception, MessageTemplate messageTemplate, ImmutableDictionary<string, LogEventPropertyValue> properties)
//        {
//            Timestamp = timestamp;
//            Level = level;
//            MessageTemplate = messageTemplate;
//            Properties = properties;
//            Exception = exception;
//        }
//    }



//    sealed class Sink : Element
//    {
//        public override void Propagate(ref EventData eventData, Emitter next)
//        {
//            Console.WriteLine(eventData.MessageTemplate.Render(eventData.Properties));
//            next.Emit(ref eventData);
//        }
//    }

//    sealed class Enricher : Element
//    {
//        readonly string _content;

//        public Enricher(string content)
//        {
//            _content = content;
//        }

//        public override void Propagate(ref EventData eventData, Emitter next)
//        {
//            var enriched = new EventData(
//                eventData.Timestamp,
//                eventData.Level,
//                eventData.Exception,
//                new MessageTemplateParser().Parse(eventData.MessageTemplate.Text + _content),
//                eventData.Properties);
//            next.Emit(ref enriched);
//        }
//    }

//    struct EventDataBuilder
//    {
//        readonly ImmutableDictionary<string, LogEventPropertyValue>.Builder _properties;

//        public DateTimeOffset Timestamp { get; }
//        public LogEventLevel Level { get; }
//        public MessageTemplate MessageTemplate { get; }
//        public Exception Exception { get; }

//        public IDictionary<string, LogEventPropertyValue> Properties => _properties;

//        EventDataBuilder(DateTimeOffset timestamp, LogEventLevel level, Exception exception, MessageTemplate messageTemplate, ImmutableDictionary<string, LogEventPropertyValue>.Builder properties)
//        {
//            Timestamp = timestamp;
//            Level = level;
//            MessageTemplate = messageTemplate;
//            _properties = properties;
//            Exception = exception;
//        }

//        public static EventDataBuilder FromEventData(ref EventData eventData, int reservedAdditionalProperties)
//        {
//            return new EventDataBuilder(
//                eventData.Timestamp,
//                eventData.Level,
//                eventData.Exception,
//                eventData.MessageTemplate,
//                eventData.Properties.ToBuilder());
//        }

//        public EventData ToEventData()
//        {
//            return new EventData(Timestamp, Level, Exception, MessageTemplate, _properties.ToImmutable());
//        }
//    }

//    interface IEventDataEnricher
//    {
//        void Enrich(ref EventDataBuilder eventDataBuilder);
//    }

//    sealed class AggregateEnricher : Element
//    {
//        readonly IEventDataEnricher[] _enrichers;

//        public AggregateEnricher(params IEventDataEnricher[] enrichers)
//        {
//            _enrichers = enrichers;
//        }

//        public override void Propagate(ref EventData eventData, Emitter next)
//        {
//            // Assume each enricher will add a property; 
//            var builder = EventDataBuilder.FromEventData(ref eventData, _enrichers.Length);
//            foreach (var enricher in _enrichers)
//            {
//                enricher.Enrich(ref builder);
//            }
//            var enriched = builder.ToEventData();
//            next.Emit(ref enriched);
//        }
//    }


//    class ScalarPropertyEnricher : IEventDataEnricher
//    {
//        readonly ScalarValue _value;
//        readonly string _name;

//        public ScalarPropertyEnricher(string name, object value)
//        {
//            _name = name;
//            _value = new ScalarValue(value);
//        }

//        public void Enrich(ref EventDataBuilder eventDataBuilder)
//        {
//            eventDataBuilder.Properties[_name] = _value;
//        }
//    }

//    class Program
//    {
//        static void Main()
//        {
//            var pipeline = PipelineBuilder.Build(
//                new Filter(true),
//                new Tap(PipelineBuilder.Build(
//                    new Enricher(" there"),
//                    new Sink())),
//                new AggregateEnricher(new ScalarPropertyEnricher("Name", "world")),
//                new Enricher(", {Name}!"),
//                new Sink());

//            var evt = new EventData(
//                DateTimeOffset.Now,
//                LogEventLevel.Information,
//                null,
//                new MessageTemplateParser().Parse("Hello"),
//                ImmutableDictionary<string, LogEventPropertyValue>.Empty);

//            pipeline.Emit(ref evt);
//        }
//    }
//}
//}

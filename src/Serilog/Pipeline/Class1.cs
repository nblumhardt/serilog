//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Serilog.Events;
//using Serilog.Parsing;

//namespace Serilog.Pipeline
//{

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

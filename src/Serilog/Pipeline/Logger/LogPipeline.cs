using System;
using Serilog.Capturing;
using Serilog.Core;
using Serilog.Events;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Enrich;
using Serilog.Pipeline.Event;
using Serilog.Pipeline.Properties;

namespace Serilog.Pipeline.Logger
{
    // This is temporary
    class LogPipeline
    {
        readonly Emitter<EventData> _emitter;
        readonly Enricher _enricher;
        readonly MessageTemplateProcessor _messageTemplateProcessor;
        readonly LogEventLevel _minimumLevel;
        readonly Action _dispose;
        readonly LoggingLevelSwitch _levelSwitch;
        readonly LevelOverrideMap _overrideMap;

        public LogPipeline(
            Emitter<EventData> emitter,
            MessageTemplateProcessor messageTemplateProcessor,
            LogEventLevel minimumLevel,
            Action dispose = null,
            LoggingLevelSwitch levelSwitch = null,
            LevelOverrideMap overrideMap = null,
            Enricher enricher = null)
        {
            _emitter = emitter ?? throw new ArgumentNullException(nameof(emitter));
            _messageTemplateProcessor = messageTemplateProcessor ?? throw new ArgumentNullException(nameof(messageTemplateProcessor));
            _minimumLevel = minimumLevel;
            _dispose = dispose;
            _levelSwitch = levelSwitch;
            _overrideMap = overrideMap;
            _enricher = enricher ?? TerminatingEnricher.Instance;
        }
        
        /// <summary>
        /// Determine if events at the specified level will be passed through
        /// to the log sinks.
        /// </summary>
        /// <param name="level">Level to check.</param>
        /// <returns>True if the level is enabled; otherwise, false.</returns>
        public bool IsEnabled(LogEventLevel level)
        {
            if ((int)level < (int)_minimumLevel)
                return false;

            return _levelSwitch == null ||
                   (int)level >= (int)_levelSwitch.MinimumLevel;
        }

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValue">Object positionally formatted into the message template.</param>
        /// <example>
        /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
        /// </example>
        [MessageTemplateFormatMethod("messageTemplate")]
        public void Information<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValue);
        }
        
        /// <summary>
        /// Write a log event with the specified level and associated exception.
        /// </summary>
        /// <param name="level">The level of the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValue">Object positionally formatted into the message template.</param>
        [MessageTemplateFormatMethod("messageTemplate")]
        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
        {
            if (!IsEnabled(level)) return;
            if (messageTemplate == null) return;

            _messageTemplateProcessor.Process(messageTemplate, new object[] {propertyValue}, out var parsedTemplate, out var boundProperties);

            var data = new EventDataBuilder(DateTimeOffset.Now, level, exception, parsedTemplate, EventPropertiesBuilder.FromElements(boundProperties, _enricher.EstimatedCount));
            Dispatch(ref data);
        }

        /// <summary>
        /// Write a log event with the specified level and associated exception.
        /// </summary>
        /// <param name="level">The level of the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        [MessageTemplateFormatMethod("messageTemplate")]
        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (!IsEnabled(level)) return;
            if (messageTemplate == null) return;

            // Catch a common pitfall when a single non-object array is cast to object[]
            if (propertyValues != null &&
                propertyValues.GetType() != typeof(object[]))
                propertyValues = new object[] { propertyValues };

            _messageTemplateProcessor.Process(messageTemplate, propertyValues, out var parsedTemplate, out var boundProperties);

            var data = new EventDataBuilder(DateTimeOffset.Now, level, exception, parsedTemplate, EventPropertiesBuilder.FromElements(boundProperties, _enricher.EstimatedCount));
            Dispatch(ref data);
        }

        public LogPipeline ForContext<TEnricher>(TEnricher enricher)
            where TEnricher: Enricher
        {
            var chain = new ChainedEnricher<TEnricher>(enricher, _enricher);
            return new LogPipeline(_emitter, _messageTemplateProcessor, _minimumLevel, null, _levelSwitch, _overrideMap, chain);
        }

        void Dispatch(ref EventDataBuilder builder)
        {
            _enricher.Enrich(ref builder, _messageTemplateProcessor);
            var data = EventDataBuilder.IntoImmutable(ref builder);
            _emitter.Emit(in data);
        }
    }
}

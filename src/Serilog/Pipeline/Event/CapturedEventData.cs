using Serilog.Pipeline.Exceptions;

namespace Serilog.Pipeline.Event
{
    class CapturedEventData
    {
        readonly EventDataBuilder _builder;

        public CapturedEventData(in EventData eventData)
        {
            _builder = eventData.ToBuilder(0);
            if (_builder.Exception != null)
                _builder.Exception = new CapturedException(_builder.Exception);
        }

        // Unsure why ReSharper thinks `ToImmutable()` is impure.
        public EventData GetEventData() => _builder.ToImmutable();
    }
}

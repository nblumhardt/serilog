using System;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Pipeline.Adapters
{
    class LogEventPropertyValueFactory : ILogEventPropertyValueFactory
    {
        readonly ILogEventPropertyFactory _propertyFactory;

        public LogEventPropertyValueFactory(ILogEventPropertyFactory propertyFactory)
        {
            _propertyFactory = propertyFactory ?? throw new ArgumentNullException(nameof(propertyFactory));
        }

        public LogEventPropertyValue CreatePropertyValue(object value, bool destructureObjects = false)
        {
            return _propertyFactory.CreateProperty("?", value, destructureObjects).Value;
        }
    }
}

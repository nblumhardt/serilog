﻿using Serilog.Core;
using Serilog.Events;
using Serilog.Pipeline.Adapters;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Filters
{
    abstract class Filter : DataPredicate, ILogEventFilter
    {
        public virtual bool IsEnabled(LogEvent logEvent)
        {
            var data = Adapt.ToEventData(logEvent);
            return IsMatch(in data);
        }
    }
}

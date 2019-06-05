using System.Collections.Generic;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Event;
using Xunit;

namespace Serilog.Tests.Pipeline.Support
{
    sealed class Collector : Emitter
    {
        public List<CapturedEventData> Items { get; } = new List<CapturedEventData>();

        public CapturedEventData Single => Assert.Single(Items);

        public override void Emit(in EventData data)
        {
            Items.Add(data.Capture());
        }
    }
}

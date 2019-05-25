using System.Collections.Generic;
using Serilog.Pipeline.Elements;
using Xunit;

namespace Serilog.Tests.Pipeline.Support
{
    sealed class Collector<T> : Emitter<T> where T : struct
    {
        public List<T> Items { get; } = new List<T>();

        public T Single => Assert.Single(Items);

        public override void Emit(in T data)
        {
            Items.Add(data);
        }
    }
}

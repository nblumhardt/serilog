using Serilog.Pipeline.Elements;
using Serilog.Tests.Pipeline.Support;
using Xunit;

namespace Serilog.Tests.Pipeline.Elements
{
    public class PipelineBuilderTests
    {
        [Fact]
        public void ElementsAreInvokedInOrder()
        {
            var collector = new Collector<int>();

            var pipeline = new PipelineBuilder<int>()
                .Add(new TransformElement<int>((in int n) => n + 2))
                .Add(new TransformElement<int>((in int n) => n * 2))
                .Tap(collector)
                .Build();

            pipeline.Emit(5);

            Assert.Equal(14, collector.Single);
        }
    }
}

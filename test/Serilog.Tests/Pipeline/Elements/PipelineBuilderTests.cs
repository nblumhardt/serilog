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

            var pipeline = PipelineBuilder.Build(
                new TransformElement<int>((in int n) => n + 2),
                new TransformElement<int>((in int n) => n * 2),
                new TapElement<int, Collector<int>>(collector)
            );

            pipeline.Emit(5);

            Assert.Equal(14, collector.Single);
        }
    }
}

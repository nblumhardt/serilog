// Copyright 2013-2016 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using Serilog.Capturing;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Events;
using Serilog.PerformanceTests.Support;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Enrich;
using Serilog.Pipeline.Event;
using Serilog.Pipeline.Logger;

namespace Serilog.PerformanceTests
{
    /// <summary>
    /// Tests the cost of writing through the logging pipeline.
    /// </summary>
    [MemoryDiagnoser]
    public class PipelineBenchmark
    {
        Exception _exception;

        Logger _log;
        ILogger _logEnriched;

        LogPipeline _pipeline, _pipelineEnriched;

        [GlobalSetup]
        public void Setup()
        {
            _exception = new Exception("An Error");
            _log = new LoggerConfiguration()
                .WriteTo.Sink(new NullSink())
                .CreateLogger();
            _logEnriched = _log.ForContext<PipelineBenchmark>().ForContext("RequestId", "12345");

            var emitter = new PipelineBuilder<EventData>()
                .Tap(new NullSink())
                .Build();

            var converter = new PropertyValueConverter(
                10,
                int.MaxValue,
                int.MaxValue,
                Enumerable.Empty<Type>(),
                Enumerable.Empty<IDestructuringPolicy>(),
                false);

            var processor = new MessageTemplateProcessor(converter);

            _pipeline = new LogPipeline(
                emitter,
                processor,
                LogEventLevel.Information);

            var enriched = PipelineBuilder<EventData>.Link(new SafeEnricher<FixedPropertyEnricher>(new FixedPropertyEnricher(new EventProperty("RequestId", new ScalarValue("12345"))), converter),
                PipelineBuilder<EventData>.Link(new SafeEnricher<FixedPropertyEnricher>(new FixedPropertyEnricher(new EventProperty("SourceContext", new ScalarValue(typeof(PipelineBenchmark).FullName))), converter), emitter));
            _pipelineEnriched = new LogPipeline(
                enriched,
                processor,
                LogEventLevel.Information);
        }

        [Benchmark(Baseline = true)]
        public void EmitLogEvent()
        {
            _log.Information(_exception, "Hello, {Name}!", "World");
        }

        [Benchmark]
        public void EmitPipelineEvent()
        {
            _pipeline.Information(_exception, "Hello, {Name}!", "World");
        }

        [Benchmark]
        public void EmitEnrichedLogEvent()
        {
            _logEnriched.Information(_exception, "Hello, {Name}!", "World");
        }

        [Benchmark]
        public void EmitEnrichedPipelineEvent()
        {
            _pipelineEnriched.Information(_exception, "Hello, {Name}!", "World");
        }
    }
}

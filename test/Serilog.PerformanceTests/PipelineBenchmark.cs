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
using Serilog.Capturing;
using Serilog.Events;
using Serilog.PerformanceTests.Support;

namespace Serilog.PerformanceTests
{
    /// <summary>
    /// Tests the cost of writing through the logging pipeline.
    /// </summary>
    [MemoryDiagnoser]
    public class PipelineBenchmark
    {
        readonly ILogger _log;
        readonly Exception _exception;
        readonly PreparedMessageTemplate<string> _prepared;

        const string Template = "Hello, {Name}!";

        public PipelineBenchmark()
        { 
            _exception = new Exception("An Error");
            _log = new LoggerConfiguration()
                .WriteTo.Sink(new NullSink())
                .CreateLogger();

            // Ensure template is cached
            _log.Information(_exception, Template, "World");

            _prepared = new PreparedMessageTemplate<string>(Template);
        }

        [Benchmark(Baseline = true)]
        public void EmitLogEvent()
        {
            _log.Information(_exception, Template, "World");
        }

        [Benchmark]
        public void EmitPreparedLogEvent()
        {
            _prepared.WriteEvent(_log, LogEventLevel.Information, _exception, "World");
        }
    }
}

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
using Serilog.Capturing;
using Serilog.Events;

namespace Serilog.PerformanceTests
{
    /// <summary>
    /// Tests the cost of creating log events from a set of parameters.
    /// </summary>
    [MemoryDiagnoser]
    public class BindingBenchmark
    {
        readonly ILogger _log;
        readonly PreparedMessageTemplate<string, string, int> _prepared;

        const string Template = "Template with {One}, {Two} and {Three}";

        public BindingBenchmark()
        { 
            _log = new LoggerConfiguration().CreateLogger();

            // Ensure template is cached
            _log.Information(Template, "One", "Two", 3);

            _prepared = new PreparedMessageTemplate<string, string, int>(Template);
        }

        [Benchmark(Baseline = true)]
        public void EmitLogEvent()
        {
            _log.Information(Template, "One", "Two", 3);
        }

        [Benchmark]
        public void EmitPreparedLogEvent()
        {
            _prepared.WriteEvent(_log, LogEventLevel.Information, "One", "Two", 3);
        }
    }
}

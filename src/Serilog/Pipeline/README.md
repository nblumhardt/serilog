# Pipeline v2

The original Serilog pipeline, configured through `LoggerConfiguration` and implemented by `Logger` and its supporting classes, has been pleasantly successful and imposes very few limitations on what can be built with Serilog. In recent years, however, groundbreaking projects like Kestrel have led the .NET library ecosystem to begin ratcheting down the use of dynamically-allocated memory in order to improve overall application performance. As a good citizen, Serilog will ideally follow suit and remove spurious allocations from its hot paths.

Which allocations? The fundamental candidates for improvement are `LogEvent`, `LogEventProperty`, and the `LogEventPropertyValue` class hierarchy.

## Functionality

## Performance

 * Substantially reduce the allocations required for logging through Serilog

## Compatibility

To be successful, the design should ensure there is no ecosystem split.

By defining ABCs for new pipeline components, all new components (sinks, enrichers, filters) work by default with existing code through unbroken "classic" `ILogEventSink`, `ILogEventEnricher`, `ILogEventFilter` interfaces. Production-quality components can override these 

To ensure that there is no unavoidable performance penalty when falling back to classic interfaces (new components can implement these directly), i.e. when an existing classic component needs to be plugged into an otherwise-all-new pipeline, the whole pipeline will work in classic mode, without mapping, and using a single `LogEvent` allocation across all

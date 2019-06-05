using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Elements
{
    abstract class DataPredicate
    {
        public abstract bool IsMatch(in EventData data);
    }
}

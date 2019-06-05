using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Elements
{
    abstract class Emitter
    {
        public abstract void Emit(in EventData data);
    }
}

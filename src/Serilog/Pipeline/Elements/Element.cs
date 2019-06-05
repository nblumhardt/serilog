using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Elements
{
    abstract class Element
    {
        public abstract void Propagate(in EventData data, Emitter next);
    }
}

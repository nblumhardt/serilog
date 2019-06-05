using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Elements
{
    sealed class Terminator : Emitter
    {
        public override void Emit(in EventData data)
        {
        }
    }
}

using System;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Elements
{
    sealed class TapElement<TEmitter> : Element
        where TEmitter : Emitter
    {
        readonly TEmitter _emitter;

        public TapElement(TEmitter emitter)
        {
            _emitter = emitter ?? throw new ArgumentNullException(nameof(emitter));
        }
        
        public override void Propagate(in EventData data, Emitter next)
        {
            // Generic TEmitter means this should be a direct (non-virtual) call if TEmitter is sealed.
            _emitter.Emit(in data);
            next.Emit(in data);
        }
    }
}

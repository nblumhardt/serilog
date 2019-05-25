using System;

namespace Serilog.Pipeline.Elements
{
    sealed class TapElement<T, TEmitter> : Element<T>
        where T : struct
        where TEmitter : Emitter<T>
    {
        readonly TEmitter _emitter;

        public TapElement(TEmitter emitter)
        {
            _emitter = emitter ?? throw new ArgumentNullException(nameof(emitter));
        }
        
        public override void Propagate(in T data, Emitter<T> next)
        {
            // Generic TEmitter means this should be a direct (non-virtual) call if TEmitter is sealed.
            _emitter.Emit(in data);
            next.Emit(in data);
        }
    }
}

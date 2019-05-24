using System;

namespace Serilog.Pipeline.Elements
{
    sealed class Tap<T, TEmitter> : Element<T>
        where T : struct
        where TEmitter : Emitter<T>
    {
        readonly TEmitter _emitter;

        public Tap(TEmitter emitter)
        {
            _emitter = emitter ?? throw new ArgumentNullException(nameof(emitter));
        }
        
        public override void Propagate(in T data, Emitter<T> next)
        {
            // Generic TEmitter means this will often be a direct (non-virtual) call.
            _emitter.Emit(in data);
            next.Emit(in data);
        }
    }
}
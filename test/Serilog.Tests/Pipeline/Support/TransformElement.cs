using System;
using Serilog.Pipeline.Elements;

namespace Serilog.Tests.Pipeline.Support
{
    sealed class TransformElement<T> : Element<T> where T : struct
    {
        readonly Transform _transform;

        public delegate T Transform(in T data);

        public TransformElement(Transform transform)
        {
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        }

        public override void Propagate(in T data, Emitter<T> next)
        {
            next.Emit(_transform(in data));
        }
    }
}

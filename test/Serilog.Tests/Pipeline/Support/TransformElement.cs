using System;
using Serilog.Pipeline.Elements;
using Serilog.Pipeline.Event;

namespace Serilog.Tests.Pipeline.Support
{
    sealed class TransformElement : Element
    {
        readonly Transform _transform;

        public delegate EventData Transform(in EventData data);

        public TransformElement(Transform transform)
        {
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        }

        public override void Propagate(in EventData data, Emitter next)
        {
            next.Emit(_transform(in data));
        }
    }
}

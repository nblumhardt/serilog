using System;

namespace Serilog.Pipeline.Elements
{
    sealed class Link<T> : Emitter<T>
        where T: struct
    {
        readonly Element<T> _element;
        readonly Emitter<T> _next;

        public Link(Element<T> element, Emitter<T> next)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public override void Emit(in T data)
        {
            _element.Propagate(in data, _next);
        }
    }
}
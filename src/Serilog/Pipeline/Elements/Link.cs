using System;

namespace Serilog.Pipeline.Elements
{
    sealed class Link<T, TElement> : Emitter<T>
        where T: struct
        where TElement: Element<T>
    {
        readonly TElement _element;
        readonly Emitter<T> _next;

        public Link(TElement element, Emitter<T> next)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public override void Emit(in T data)
        {
            // Generic _element should in some cases (sealed TElement) be able to avoid virtual dispatch.
            _element.Propagate(in data, _next);
        }
    }
}

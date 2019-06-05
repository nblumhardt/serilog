using System;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Elements
{
    sealed class Link<TElement> : Emitter
        where TElement: Element
    {
        readonly TElement _element;
        readonly Emitter _next;

        public Link(TElement element, Emitter next)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public override void Emit(in EventData data)
        {
            // Generic _element should in some cases (sealed TElement) be able to avoid virtual dispatch.
            _element.Propagate(in data, _next);
        }
    }
}

using System;
using System.Collections.Generic;

namespace Serilog.Pipeline.Elements
{
    class PipelineBuilder
    {
        readonly List<Element> _elements = new List<Element>();

        public static Link<TElement> Link<TElement>(TElement head, Emitter next)
            where TElement: Element
        {
            if (head == null) throw new ArgumentNullException(nameof(head));
            if (next == null) throw new ArgumentNullException(nameof(next));
            return new Link<TElement>(head, next);
        }

        public PipelineBuilder Add(Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            _elements.Add(element);
            return this;
        }

        public PipelineBuilder Tap<TEmitter>(TEmitter tap)
            where TEmitter : Emitter
        {
            if (tap == null) throw new ArgumentNullException(nameof(tap));
            return Add(new TapElement<TEmitter>(tap));
        }

        public Emitter Build()
        {
            Emitter head = new Terminator();
            for (var i = _elements.Count - 1; i >= 0; --i)
                head = Link(_elements[i], head);

            return head;
        }
    }
}

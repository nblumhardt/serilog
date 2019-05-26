using System;
using System.Collections.Generic;

namespace Serilog.Pipeline.Elements
{
    class PipelineBuilder<T>
        where T: struct
    {
        readonly List<Element<T>> _elements = new List<Element<T>>();

        public static Link<T, TElement> Link<TElement>(TElement head, Emitter<T> next)
            where TElement: Element<T>
        {
            if (head == null) throw new ArgumentNullException(nameof(head));
            if (next == null) throw new ArgumentNullException(nameof(next));
            return new Link<T, TElement>(head, next);
        }

        public PipelineBuilder<T> Add(Element<T> element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            _elements.Add(element);
            return this;
        }

        public PipelineBuilder<T> Tap<TEmitter>(TEmitter tap)
            where TEmitter : Emitter<T>
        {
            if (tap == null) throw new ArgumentNullException(nameof(tap));
            return Add(new TapElement<T, TEmitter>(tap));
        }

        public Emitter<T> Build()
        {
            Emitter<T> head = new Terminator<T>();
            for (var i = _elements.Count - 1; i >= 0; --i)
                head = Link(_elements[i], head);

            return head;
        }
    }
}

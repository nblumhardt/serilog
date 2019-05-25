using System;

namespace Serilog.Pipeline.Elements
{
    static class PipelineBuilder
    {
        public static Link<T, TElement> Link<T, TElement>(TElement head, Emitter<T> next)
            where T: struct
            where TElement: Element<T>
        {
            if (head == null) throw new ArgumentNullException(nameof(head));
            if (next == null) throw new ArgumentNullException(nameof(next));
            return new Link<T, TElement>(head, next);
        }

        public static Emitter<T> Build<T>(params Element<T>[] elements)
            where T : struct
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));

            Emitter<T> head = new Terminator<T>();
            for (var i = elements.Length - 1; i >= 0; --i)
                head = Link(elements[i], head);

            return head;
        }
    }
}

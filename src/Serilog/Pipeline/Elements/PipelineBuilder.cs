using System;

namespace Serilog.Pipeline.Elements
{
    static class PipelineBuilder
    {
        public static Emitter<T> Build<T>(params Element<T>[] elements)
            where T : struct
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));

            Emitter<T> head = new Terminator<T>();

            for (var i = elements.Length - 1; i >= 0; --i)
                head = new Link<T>(elements[i], head);

            return head;
        }
    }
}

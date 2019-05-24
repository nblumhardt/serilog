using System;

namespace Serilog.Pipeline.Elements
{
    sealed class Filter<T> : Element<T>
        where T: struct
    {
        readonly DataPredicate<T> _condition;

        public Filter(DataPredicate<T> condition) 
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public override void Propagate(in T data, Emitter<T> next)
        {
            if (_condition(data))
                next.Emit(in data);
        }
    }
}
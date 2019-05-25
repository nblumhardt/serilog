using System;

namespace Serilog.Pipeline.Elements
{
    sealed class FilterElement<T, P> : Element<T>
        where T: struct
        where P: DataPredicate<T>
    {
        readonly P _condition;

        public FilterElement(P condition) 
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public override void Propagate(in T data, Emitter<T> next)
        {
            if (_condition.IsMatch(data))
                next.Emit(in data);
        }
    }
}

using System;
using Serilog.Pipeline.Event;

namespace Serilog.Pipeline.Elements
{
    sealed class FilterElement<P> : Element
        where P: DataPredicate
    {
        readonly P _condition;

        public FilterElement(P condition) 
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public override void Propagate(in EventData data, Emitter next)
        {
            if (_condition.IsMatch(data))
                next.Emit(in data);
        }
    }
}

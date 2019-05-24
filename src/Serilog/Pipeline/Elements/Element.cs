namespace Serilog.Pipeline.Elements
{
    abstract class Element<T>
        where T: struct
    {
        public abstract void Propagate(in T data, Emitter<T> next);
    }
}
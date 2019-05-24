namespace Serilog.Pipeline.Elements
{
    abstract class Emitter<T>
        where T: struct
    {
        public abstract void Emit(in T data);
    }
}
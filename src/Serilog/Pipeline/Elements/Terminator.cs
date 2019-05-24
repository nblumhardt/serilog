namespace Serilog.Pipeline.Elements
{
    sealed class Terminator<T> : Emitter<T>
        where T: struct
    {
        public override void Emit(in T data)
        {
        }
    }
}
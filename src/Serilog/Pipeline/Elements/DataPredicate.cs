namespace Serilog.Pipeline.Elements
{
    abstract class DataPredicate<T>
        where T: struct
    {
        public abstract bool IsMatch(in T data);
    }
}

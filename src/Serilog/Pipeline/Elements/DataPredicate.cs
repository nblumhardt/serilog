namespace Serilog.Pipeline.Elements
{
    delegate bool DataPredicate<T>(in T data) where T : struct;
}

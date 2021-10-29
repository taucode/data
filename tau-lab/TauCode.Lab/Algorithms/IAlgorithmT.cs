namespace TauCode.Lab.Algorithms
{
    public interface IAlgorithm<TInput, out TOutput> : IAlgorithm
    {
        TInput Input { get; set; }
        TOutput Output { get; }
    }
}
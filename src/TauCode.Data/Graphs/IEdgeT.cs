namespace TauCode.Data.Graphs
{
    public interface IEdge<T> : IEdge
    {
        T Data { get; set; }
    }
}

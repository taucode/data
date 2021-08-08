namespace TauCode.Lab.Data.Graphs
{
    public interface IEdge<T> : IEdge
    {
        T Data { get; set; }
    }
}

namespace TauCode.Lab.Data.Graphs
{
    public interface IVertex<T> : IVertex
    {
        T Data{ get; set; }
    }
}

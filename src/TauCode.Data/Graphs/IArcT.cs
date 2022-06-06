namespace TauCode.Data.Graphs
{
    public interface IArc<T> : IArc
    {
        T Data { get; set; }
    }
}

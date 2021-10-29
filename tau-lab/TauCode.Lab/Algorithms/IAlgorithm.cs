using System.Threading;
using System.Threading.Tasks;

namespace TauCode.Lab.Algorithms
{
    public interface IAlgorithm
    {
        void Run();
        Task RunAsync(CancellationToken cancellationToken = default);
    }
}

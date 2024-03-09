
namespace TaskManagerApi.Facades
{
    public interface IPerformanceCounterFacade
    {
        Func<float> GetCounterNextValues { get; init; }

        string? Name { get; }
    }
}
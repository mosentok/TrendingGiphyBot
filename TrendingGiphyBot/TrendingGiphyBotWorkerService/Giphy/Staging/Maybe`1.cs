namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class Maybe<T>
{
    public bool Success { get; }
    public T? Result { get; }

    public Maybe() { }

    public Maybe(T result)
    {
        Result = result;
        Success = true;
    }
}
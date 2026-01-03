namespace TrendingGiphyBotWorkerService
{
    public class ThisShouldBeImpossibleException : Exception
    {
        public ThisShouldBeImpossibleException() { }
        public ThisShouldBeImpossibleException(string? message) : base(message) { }
        public ThisShouldBeImpossibleException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService;

public class ThisShouldBeImpossibleException : Exception
{
    public ThisShouldBeImpossibleException() { }
    public ThisShouldBeImpossibleException(string? message) : base(message) { }
    public ThisShouldBeImpossibleException(string? message, Exception? innerException) : base(message, innerException) { }

    public static void ThrowIf(
        [DoesNotReturnIf(true)] bool condition,
        [CallerArgumentExpression(nameof(condition))] string? expression = null)
    {
        if (condition)
            throw new ThisShouldBeImpossibleException($"Condition failed: {expression}");
    }
}
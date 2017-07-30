using Discord.Commands;

namespace TrendingGiphyBot.Containers
{
    class ErrorResult
    {
        internal CommandError? Error { get; set; }
        internal string ErrorReason { get; set; }
        internal bool IsSuccess { get; set; }
        internal ErrorResult(IResult result) : this(result.Error, result.ErrorReason, result.IsSuccess) { }
        internal ErrorResult(CommandError? error, string errorReason, bool isSuccess)
        {
            Error = error;
            ErrorReason = errorReason;
            IsSuccess = isSuccess;
        }
    }
}

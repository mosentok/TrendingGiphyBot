using Discord.Commands;

namespace TrendingGiphyBot.Containers
{
    public class ErrorResult
    {
        public CommandError? Error { get; set; }
        public string ErrorReason { get; set; }
        public bool IsSuccess { get; set; }
        public ErrorResult(IResult result) : this(result.Error, result.ErrorReason, result.IsSuccess) { }
        public ErrorResult(CommandError? error, string errorReason, bool isSuccess)
        {
            Error = error;
            ErrorReason = errorReason;
            IsSuccess = isSuccess;
        }
    }
}

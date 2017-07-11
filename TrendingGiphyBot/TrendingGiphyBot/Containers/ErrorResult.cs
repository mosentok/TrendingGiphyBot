using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrendingGiphyBot.Containers
{
    public class ErrorResult
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CommandError? Error { get; set; }
        public string ErrorReason { get; set; }
        public bool IsSuccess { get; set; }
        public ErrorResult(IResult result)
        {
            Error = result.Error;
            ErrorReason = result.ErrorReason;
            IsSuccess = IsSuccess;
        }
    }
}

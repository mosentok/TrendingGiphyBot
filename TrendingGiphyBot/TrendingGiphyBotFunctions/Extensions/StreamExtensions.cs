using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace TrendingGiphyBotFunctions.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadToEndAsync(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return await reader.ReadToEndAsync();
        }
        public static async Task<T> ReadToEndAsync<T>(this Stream stream) where T : class, new()
        {
            var content = await stream.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}

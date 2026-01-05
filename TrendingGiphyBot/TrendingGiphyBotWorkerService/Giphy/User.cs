using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy;

public class User
{
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonPropertyName("banner_image")]
    public string BannerImage { get; set; }

    [JsonPropertyName("profile_url")]
    public string ProfileUrl { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("is_verified")]
    public bool IsVerified { get; set; }
}

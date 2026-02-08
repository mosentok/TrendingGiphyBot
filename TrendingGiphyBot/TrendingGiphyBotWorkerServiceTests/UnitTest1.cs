using NUnit.Framework;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerServiceTests;

public class GifSourceTests
{
    [Test]
    public void ChannelSettings_Defaults_To_Giphy()
    {
        var cs = new ChannelSettingsModel();

        Assert.That(cs.GifSource, Is.EqualTo(GifSourceKind.Giphy));
    }

    [Test]
    public void GifSourceKind_Flags_Combine_And_HasFlag_Works()
    {
        var combined = GifSourceKind.Giphy | GifSourceKind.Klipy;

        Assert.That(combined.HasFlag(GifSourceKind.Giphy));
        Assert.That(combined.HasFlag(GifSourceKind.Klipy));
        Assert.That((int)combined, Is.EqualTo(3));
    }
}

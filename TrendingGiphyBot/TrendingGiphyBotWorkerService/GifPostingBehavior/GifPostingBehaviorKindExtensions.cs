using System.Diagnostics.CodeAnalysis;

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

[SuppressMessage("", "S2325", Justification = "SonarQube hasn't been updated to handle the extensions keyword. SonarQube thinks these methods don't access instance data.")]
public static class GifPostingBehaviorKindExtensions
{
    extension(GifPostingBehaviorKind kind)
    {
        public int AsInt() => (int)kind;
    }
}

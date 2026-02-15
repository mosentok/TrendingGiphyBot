namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public static class GifPostingBehaviorKindExtensions
{
    extension(GifPostingBehaviorKind kind)
    {
        public int AsInt() => (int)kind;
    }
}

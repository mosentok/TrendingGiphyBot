namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public static class GifPostingBehaviorKindExtensions
{
    extension(GifPostingBehaviorKind kind)
    {
        public int AsInt() => (int)kind;

        public string GetDescription() =>
            kind switch
            {
                GifPostingBehaviorKind.None => "None",
                GifPostingBehaviorKind.TrendingGifsOnly => "Trending Gifs Only",
                GifPostingBehaviorKind.TrendingGifsWithRandomGifs => "Trending Gifs with Random Gifs",
                _ => throw new ThisShouldBeImpossibleException()
            };
    }
}

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public static class GifPostingBehaviorKindExtensions
{
    extension(GifPostingBehaviorKind kind)
    {
        public string GetDescription() =>
            kind switch
            {
                // TODO throw an exception if value is None
                GifPostingBehaviorKind.None => "None",
                GifPostingBehaviorKind.TrendingGifsOnly => "Trending Gifs Only",
                GifPostingBehaviorKind.TrendingGifsWithRandomGifs => "Trending Gifs with Random Gifs",
                _ => throw new ThisShouldBeImpossibleException()
            };
    }
}

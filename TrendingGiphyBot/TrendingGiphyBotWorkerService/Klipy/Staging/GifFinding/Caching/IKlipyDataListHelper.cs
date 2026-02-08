using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

public interface IKlipyDataListHelper
{
    void TrimToMaxSize(List<KlipyData> listToTrim, List<KlipyData> itemsToAdd, int maxCount);
}
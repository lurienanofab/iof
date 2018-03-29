using IOF.Models;

namespace IOF
{
    public interface ISearchService
    {
        SearchResult<OrderSearchItem> OrderSearch(OrderSearchArgs args);
        SearchResult<PurchaserSearchItem> PurchaserSearch(PurchaserSearchArgs args);
    }
}

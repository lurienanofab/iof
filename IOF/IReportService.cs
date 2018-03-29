using IOF.Models;
using System.Collections.Generic;

namespace IOF
{
    public interface IReportService
    {
        IEnumerable<ItemReportItem> GetItemReport(int itemId);
        IEnumerable<StoreManagerReportItem> GetStoreManagerReport();
    }
}

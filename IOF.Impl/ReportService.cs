using System;
using IOF.Models;
using LNF.Repository;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ordering = LNF.Repository.Ordering;

namespace IOF.Impl
{
    public class ReportService : IReportService
    {
        public IEnumerable<ItemReportItem> GetItemReport(int itemId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                var dt = dba
                    .ApplyParameters(new { Action = "ItemReport", ItemID = itemId })
                    .FillDataTable("IOF.dbo.spReport_Select");

                var result = dt.AsEnumerable().Select(x => new ItemReportItem()
                {
                    Year = x.Field<int>("Year"),
                    Month = x.Field<int>("MonthNum"),
                    MonthName = x.Field<string>("MonthName"),
                    TotalUnit = x.Field<double>("TotalUnit"),
                    TotalCost = x.Field<double>("TotalCost")
                });

                return result;
            }
        }

        public IEnumerable<StoreManagerReportItem> GetStoreManagerReport()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                var dt = dba.FillDataTable("IOF.dbo.Report_StoreManager");

                var result = dt.AsEnumerable().Select(x => new StoreManagerReportItem()
                {
                    ItemID = x.Field<int>("ItemID"),
                    Description = x.Field<string>("Description"),
                    VendorID = x.Field<int>("VendorID"),
                    VendorName = x.Field<string>("VendorName"),
                    UnitPrice = Convert.ToDouble(x.Field<decimal>("UnitPrice")),
                    Unit = x.Field<string>("Unit"),
                    LastOrdered = x.Field<DateTime?>("LastOrdered"),
                    StoreItemID = x.Field<int?>("StoreItemID"),
                    StoreDescription = x.Field<string>("StoreDescription"),
                    StorePackagePrice = x.Field<double?>("StorePackagePrice"),
                    StorePackageQuantity = x.Field<int?>("StorePackageQty"),
                    StoreUnitPrice = x.Field<double?>("StoreUnitPrice"),
                    LastPurchased = x.Field<DateTime?>("LastPurchased")
                });

                return result;
            }
        }
    }
}

using IOF.Models;
using LNF;
using LNF.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ordering = LNF.Impl.Repository.Ordering;

namespace IOF.Impl
{
    public class SearchService : ISearchService
    {
        public IProvider Provider { get; }
        public IContext Context { get; }
        public ISession DataSession => Provider.DataAccess.Session;

        public SearchService(IProvider provider, IContext context)
        {
            Provider = provider;
            Context = context;
        }

        private bool UseSession()
        {
            if (Context.GetSessionValue("UseSession") == null)
                return false;

            if (Context.GetSessionValue("UseSession") is bool)
                return (bool)Context.GetSessionValue("UseSession");

            if (bool.TryParse(Context.GetSessionValue("UseSession").ToString(), out bool result))
                return result;

            return false;
        }

        public void SetSession(IEnumerable<OrderSearchItem> items)
        {
            Context.SetSessionValue("PurchaseOrderSearchItems", items);
        }

        public SearchResult<OrderSearchItem> OrderSearch(OrderSearchArgs args)
        {
            if (UseSession())
                return GetOrderResultFromSession(args);
            else
                return GetOrderResultFromDatabase(args);
        }

        public SearchResult<PurchaserSearchItem> PurchaserSearch(PurchaserSearchArgs args)
        {
            return GetPurchaserResultFromDatabase(args);
        }

        private SearchResult<OrderSearchItem> GetOrderResultFromDatabase(OrderSearchArgs args)
        {
            var builder = DataSession.QueryBuilder<Ordering.PurchaseOrderSearch>();

            var statusIds = args.GetStatusIds();

            if (statusIds.Length > 0)
                builder.Where(builder.Restriction(x => x.StatusID).In(statusIds));

            int recordsTotal = builder.Count();

            PurchaseOrderSearchUtility.SetSearch(builder, args);

            if (args.StartDate.HasValue)
                builder.Where(x => x.CreatedDate >= args.StartDate.Value);

            if (args.EndDate.HasValue)
                builder.Where(x => x.CreatedDate < args.EndDate.Value);

            //IncludeMyself comes from a checkbox
            //ClientID comes from the logged in user
            int clientId = (args.IncludeSelf) ? args.ClientID : -999;

            //OtherClientID comes from a dropdownlist where "View All" = -1
            int otherClientId = (args.OtherClientID > 0) ? args.OtherClientID : -999;

            if (otherClientId > 0)
                builder.Where(builder.Restriction(x => x.ClientID).In(new[] { clientId, otherClientId }));

            if (!string.IsNullOrEmpty(args.VendorName))
                builder.Where(builder.Restriction(x => x.CleanVendorName).Contains(PurchaseOrderSearchUtility.CleanString(args.VendorName)));

            if (args.VendorID > 0)
                builder.Where(x => x.VendorID == args.VendorID);

            if (!string.IsNullOrEmpty(args.Keywords))
                builder.Where(builder.Restriction(x => x.Description).Contains(args.Keywords));

            if (!string.IsNullOrEmpty(args.PartNumber))
                builder.Where(builder.Restriction(x => x.PartNum).Contains(args.PartNumber));

            if (args.POID > 0)
                builder.Where(x => x.POID == args.POID);

            if (!string.IsNullOrEmpty(args.ShortCode))
                builder.Where(builder.Restriction(x => x.ShortCode).Contains(args.ShortCode));

            int recordsFiltered = builder.Count();

            PurchaseOrderSearchUtility.SetOrder(builder, args);

            IList<Ordering.PurchaseOrderSearch> data;

            if (args.Length > 0)
                data = builder.Skip(args.Start).Take(args.Length).List();
            else
                data = builder.Skip(args.Start).List();

            return new SearchResult<OrderSearchItem>()
            {
                Draw = args.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = PurchaseOrderSearchUtility.CreateOrderItems(data, args.DisplayOption).ToArray()
            };
        }

        private SearchResult<OrderSearchItem> GetOrderResultFromSession(OrderSearchArgs args)
        {
            IEnumerable<OrderSearchItem> items;

            items = GetItemsFromSession();

            if (items == null)
            {
                items = GetItemsFromDatabase(args);
                SetSession(items);
            }

            int recordsTotal = items.Count();

            items = Search(items, args);

            if (args.StartDate.HasValue)
                items = items.Where(x => x.CreatedDate >= args.StartDate.Value);

            if (args.EndDate.HasValue)
                items = items.Where(x => x.CreatedDate < args.EndDate.Value);

            //IncludeMyself comes from a checkbox
            //ClientID comes from the logged in user
            int clientId = (args.IncludeSelf) ? args.ClientID : -999;

            //OtherClientID comes from a dropdownlist where "View All" = -1
            int otherClientId = (args.OtherClientID > 0) ? args.OtherClientID : -999;

            if (otherClientId > 0)
                items = items.Where(x => new[] { clientId, otherClientId }.Contains(x.ClientID));

            if (!string.IsNullOrEmpty(args.VendorName))
                items = items.Where(x => PurchaseOrderSearchUtility.CleanString(x.VendorName).Contains(PurchaseOrderSearchUtility.CleanString(args.VendorName)));

            if (args.VendorID > 0)
                items = items.Where(x => x.VendorID == args.VendorID);

            if (!string.IsNullOrEmpty(args.Keywords))
                items = items.Where(x => x.Description.Contains(args.Keywords));

            if (!string.IsNullOrEmpty(args.PartNumber))
                items = items.Where(x => x.PartNum.Contains(args.PartNumber));

            if (args.POID > 0)
                items = items.Where(x => x.POID == args.POID);

            if (!string.IsNullOrEmpty(args.ShortCode))
                items = items.Where(x => x.ShortCode.Contains(args.ShortCode));

            int recordsFiltered = items.Count();

            items = Order(items, args);

            var take = args.Length > 0 ? args.Length : 10; // default is 10 rows

            OrderSearchItem[] data;

            if (args.Length > 0)
                data = items.Skip(args.Start).Take(args.Length).ToArray();
            else
                data = items.Skip(args.Start).ToArray();

            return new SearchResult<OrderSearchItem>()
            {
                Draw = args.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }

        public SearchResult<PurchaserSearchItem> GetPurchaserResultFromDatabase(PurchaserSearchArgs args)
        {
            // claimStatus:
            //      All = 0
            //      Unclaimed = 1
            //      Claimed = 2
            //      Claimedby = 3

            // orderStatus:
            //      NotSpecified = 0
            //      Unordered = 1
            //      Ordered = 2

            var query = DataSession.Query<Ordering.PurchaserSearch>();

            var statusIds = args.GetStatusIds();

            if (statusIds != null && statusIds.Length > 0)
                query = query.Where(x => statusIds.Contains(x.StatusID));

            int totalRecords = query.Count();

            if (args.StartDate.HasValue)
                query = query.Where(x => x.CreatedDate >= args.StartDate.Value);

            if (args.EndDate.HasValue)
                query = query.Where(x => x.CreatedDate < args.EndDate.Value);

            switch (args.ClaimStatus)
            {
                case PurchaserClaimStatus.Unclaimed:
                    query = query.Where(x => !x.PurchaserID.HasValue);
                    break;
                case PurchaserClaimStatus.Claimed:
                    query = query.Where(x => x.PurchaserID.HasValue);
                    break;
                case PurchaserClaimStatus.ClaimedBy:
                    query = query.Where(x => x.PurchaserID == args.PurchaserClientID);
                    break;
            }

            switch (args.OrderStatus)
            {
                case PurchaserOrderStatus.Unordered:
                    query = query.Where(x => (x.RealPO == null || x.RealPO == ""));
                    break;
                case PurchaserOrderStatus.Ordered:
                    query = query.Where(x => (x.RealPO != null && x.RealPO != ""));
                    break;
            }

            if (args.PurchaserClientID > 0 && args.ClaimStatus == PurchaserClaimStatus.All)
                query = query.Where(x => x.PurchaserID == args.PurchaserClientID);

            if (args.CreatorClientID > 0)
                query = query.Where(x => x.ClientID == args.CreatorClientID);

            if (args.POID > 0)
                query = query.Where(x => x.POID == args.POID);

            if (!string.IsNullOrEmpty(args.RealPO))
                query = query.Where(x => x.RealPO.Contains(args.RealPO));

            int recordsFiltered = query.Count();

            if (args.Length > 0)
                query = query.Skip(args.Start).Take(args.Length);
            else
                query = query.Skip(args.Start);

            var data = query.Select(x => new PurchaserSearchItem()
            {
                POID = x.POID,
                StatusID = x.StatusID,
                CreatedDate = x.CreatedDate,
                ClientID = x.ClientID,
                DisplayName = x.DisplayName,
                PurchaserID = x.PurchaserID,
                PurchaserName = x.PurchaserDisplayName,
                TotalPrice = x.Total,
                RealPO = x.RealPO
            }).ToArray();

            var result = new SearchResult<PurchaserSearchItem>()
            {
                Draw = args.Draw,
                RecordsFiltered = 0,
                RecordsTotal = 0,
                Data = data
            };

            return result;
        }

        private IEnumerable<OrderSearchItem> GetItemsFromSession()
        {
            if (Context.GetSessionValue("PurchaseOrderSearchItems") != null)
                return (IEnumerable<OrderSearchItem>)Context.GetSessionValue("PurchaseOrderSearchItems");
            else
                return null;
        }

        private IEnumerable<OrderSearchItem> GetItemsFromDatabase(OrderSearchArgs args)
        {
            var query = DataSession.Query<Ordering.PurchaseOrderSearch>();

            var statusIds = args.GetStatusIds();

            if (statusIds.Length > 0)
                query = query.Where(x => statusIds.Contains(x.StatusID));

            return PurchaseOrderSearchUtility.CreateOrderItems(query, args.DisplayOption);
        }

        private static IEnumerable<OrderSearchItem> Search(IEnumerable<OrderSearchItem> items, OrderSearchArgs args)
        {
            if (args.Search == null)
                return items;

            if (string.IsNullOrEmpty(args.Search.Value))
                return items;

            if (args.Columns == null)
                return items;

            var result = items.Where(x => PurchaseOrderSearchUtility.GetSearchText(x, args).Contains(args.Search.Value.ToLower()));

            return result;
        }

        private static IEnumerable<OrderSearchItem> Order(IEnumerable<OrderSearchItem> items, OrderSearchArgs args)
        {
            if (args.Order == null)
                return items;

            if (args.Columns == null)
                return items;

            IOrderedEnumerable<OrderSearchItem> ordered = null;

            foreach (var order in args.Order)
            {
                if (args.Columns.Length > order.Column)
                {
                    var col = args.Columns[order.Column];

                    if (col.Orderable)
                    {
                        if (PurchaseOrderSearchUtility.OrderProperties.TryGetValue(col.Name, out Func<OrderSearchItem, object> fn))
                        {
                            if (order.Dir == "desc")
                                ordered = (ordered == null) ? items.OrderByDescending(fn) : ordered.ThenByDescending(fn);
                            else
                                ordered = (ordered == null) ? items.OrderBy(fn) : ordered.ThenBy(fn);
                        }
                    }
                }
            }

            if (ordered == null)
                return items;
            else
                return ordered;
        }
    }

    public static class PurchaseOrderSearchUtility
    {
        public static readonly ColumnExpressions ColumnExpressions = new ColumnExpressions()
        {

            { "POID", new ColumnExpressionItem(x => x.POID) },
            { "DisplayName", new ColumnExpressionItem(x => x.DisplayName) },
            { "ApproverDisplayName", new ColumnExpressionItem(x => x.ApproverDisplayName) },
            { "PartNum", new ColumnExpressionItem(x => x.PartNum) },
            { "Description", new ColumnExpressionItem(x => x.Description) },
            { "VendorName", new ColumnExpressionItem(x => x.VendorName) },
            { "CreatedDate", new ColumnExpressionItem(x => x.CreatedDate, x => x.CreatedDateText) },
            { "ShortCode", new ColumnExpressionItem(x => x.ShortCode) },
            { "TotalPrice", new ColumnExpressionItem(x => x.TotalPrice, x => x.TotalPriceText) },
            { "StatusName", new ColumnExpressionItem(x => x.StatusName) }
        };

        public static readonly Dictionary<string, Func<OrderSearchItem, string>> SearchProperties;

        public static readonly Dictionary<string, Func<OrderSearchItem, object>> OrderProperties;

        static PurchaseOrderSearchUtility()
        {
            SearchProperties = new Dictionary<string, Func<OrderSearchItem, string>>()
            {
                {  "POID", x => x.POID.ToString() },
                {  "DisplayName", x => x.DisplayName },
                {  "ApproverDisplayName", x => x.ApproverDisplayName },
                {  "PartNum", x => x.PartNum },
                {  "Description", x => x.Description },
                {  "VendorName", x => x.VendorName },
                {  "CreatedDate", x => x.CreatedDate.ToString("M/d/yyyy h:mm:ss tt") },
                {  "ShortCode", x => x.ShortCode },
                {  "TotalPrice", x => x.TotalPrice.ToString("C") },
                {  "StatusName", x => x.StatusName }
            };

            OrderProperties = new Dictionary<string, Func<OrderSearchItem, object>>()
            {
                {  "POID", x => x.POID },
                {  "DisplayName", x => x.DisplayName },
                {  "ApproverDisplayName", x => x.ApproverDisplayName },
                {  "PartNum", x => x.PartNum },
                {  "Description", x => x.Description },
                {  "VendorName", x => x.VendorName },
                {  "CreatedDate", x => x.CreatedDate },
                {  "ShortCode", x => x.ShortCode },
                {  "TotalPrice", x => x.TotalPrice },
                {  "StatusName", x => x.StatusName }
            };
        }

        public static string GetSearchText(OrderSearchItem item, OrderSearchArgs args)
        {
            string result = string.Empty;

            foreach (var col in args.Columns)
            {
                if (col.Searchable)
                    result += SearchProperties[col.Name](item);
            }

            return result.ToLower();
        }

        public static void SetSearch(IQueryBuilder<Ordering.PurchaseOrderSearch> builder, OrderSearchArgs args)
        {
            if (args.Search == null)
                return;

            if (string.IsNullOrEmpty(args.Search.Value))
                return;

            if (args.Columns == null)
                return;

            var disj = builder.Disjunction();

            foreach (var col in args.Columns)
            {
                if (col.Searchable)
                {
                    var item = ColumnExpressions[col.Name];
                    if (item != null)
                        disj.Add(builder.Restriction(item.Search).StartsWith(args.Search.Value));
                }
            }

            builder.And(disj);
        }

        public static void SetOrder(IQueryBuilder<Ordering.PurchaseOrderSearch> builder, OrderSearchArgs args)
        {
            if (args.Order == null)
                return;

            if (args.Columns == null)
                return;

            foreach (var order in args.Order)
            {
                if (args.Columns.Length > order.Column)
                {
                    var col = args.Columns[order.Column];

                    if (col.Orderable)
                    {
                        var item = ColumnExpressions[col.Name];

                        if (item != null)
                        {
                            if (order.Dir == "desc")
                                builder.OrderByDescending(item.Order);
                            else
                                builder.OrderBy(item.Order);
                        }
                    }
                }
            }
        }

        public static IEnumerable<OrderSearchItem> CreateOrderItems(IEnumerable<Ordering.PurchaseOrderSearch> query, OrderDisplayOption displayOption)
        {
            if (displayOption == OrderDisplayOption.Detail)
            {
                return query.Select(x => new OrderSearchItem()
                {
                    ClientID = x.ClientID,
                    POID = x.POID,
                    DisplayName = x.DisplayName,
                    ApproverDisplayName = x.ApproverDisplayName,
                    PartNum = x.PartNum,
                    Description = x.Description,
                    VendorID = x.VendorID,
                    VendorName = x.VendorName,
                    CreatedDate = x.CreatedDate,
                    ShortCode = x.ShortCode,
                    CategoryID = x.CategoryID,
                    CategoryNumber = x.CategoryNumber,
                    CategoryName = x.CategoryName,
                    TotalPrice = x.TotalPrice,
                    StatusName = x.StatusName
                }).ToList();
            }
            else
            {
                return query.GroupBy(x => new
                {
                    x.ClientID,
                    x.POID,
                    x.DisplayName,
                    x.ApproverDisplayName,
                    x.VendorID,
                    x.VendorName,
                    x.CreatedDate,
                    x.ShortCode,
                    x.TotalPrice,
                    x.StatusName
                })
                .Select(x => new OrderSearchItem()
                {
                    ClientID = x.Key.ClientID,
                    POID = x.Key.POID,
                    DisplayName = x.Key.DisplayName,
                    ApproverDisplayName = x.Key.ApproverDisplayName,
                    PartNum = null,
                    Description = null,
                    VendorID = x.Key.VendorID,
                    VendorName = x.Key.VendorName,
                    CreatedDate = x.Key.CreatedDate,
                    ShortCode = x.Key.ShortCode,
                    CategoryID = 0,
                    CategoryNumber = null,
                    CategoryName = null,
                    TotalPrice = x.Key.TotalPrice,
                    StatusName = x.Key.StatusName
                }).ToList();
            }
        }

        public static string CleanString(string input)
        {
            string result = input;

            //Remove any punctuation
            string punctuation = @"!@#$%^&*()-_=+[{]}\|;:'"",<.>/?";
            foreach (char c in punctuation.ToCharArray())
            {
                result = result.Replace(c.ToString(), string.Empty);
            }

            //Remove any spaces
            result = result.Replace(" ", string.Empty);

            //Force upper case
            result = result.ToUpper();

            return result;
        }
    }

    public class ColumnExpressions : IEnumerable<KeyValuePair<string, ColumnExpressionItem>>
    {
        private IDictionary<string, ColumnExpressionItem> _items = new Dictionary<string, ColumnExpressionItem>();

        public void Add(string name, ColumnExpressionItem item)
        {
            _items.Add(name, item);
        }

        public ColumnExpressionItem this[string name]
        {
            get
            {
                if (_items.TryGetValue(name, out ColumnExpressionItem item))
                    return item;
                else
                    return null;
            }
            set
            {
                if (_items.ContainsKey(name))
                    _items[name] = value;
                else
                    Add(name, value);
            }
        }

        public IEnumerator<KeyValuePair<string, ColumnExpressionItem>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ColumnExpressionItem
    {
        public Expression<Func<Ordering.PurchaseOrderSearch, object>> Order { get; }
        public Expression<Func<Ordering.PurchaseOrderSearch, object>> Search { get; }

        public ColumnExpressionItem(Expression<Func<Ordering.PurchaseOrderSearch, object>> both)
        {
            Order = both;
            Search = both;
        }

        public ColumnExpressionItem(Expression<Func<Ordering.PurchaseOrderSearch, object>> order, Expression<Func<Ordering.PurchaseOrderSearch, object>> search)
        {
            Order = order;
            Search = search;
        }
    }
}

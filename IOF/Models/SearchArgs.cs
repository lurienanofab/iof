using System.Linq;

namespace IOF.Models
{
    public abstract class SearchArgs
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public SearchProperty Search { get; set; }
        public OrderProperty[] Order { get; set; }
        public ColumnProperty[] Columns { get; set; }
        public string StatusIdList { get; set; }

        public int[] GetStatusIds()
        {
            if (string.IsNullOrEmpty(StatusIdList))
                return new int[0];

            var splitter = StatusIdList.Split(',');

            var result = splitter
                .Select(x => int.TryParse(x, out int i) ? (int?)i : null)
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToArray();

            return result;
        }

        public class SearchProperty
        {
            public string Value { get; set; }
            public bool Regex { get; set; }
        }

        public class OrderProperty
        {
            public int Column { get; set; }
            public string Dir { get; set; }
        }

        public class ColumnProperty
        {
            public string Data { get; set; }
            public string Name { get; set; }
            public bool Searchable { get; set; }
            public bool Orderable { get; set; }
            public SearchProperty Search { get; set; }
        }
    }
}

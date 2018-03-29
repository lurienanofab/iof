using System.Collections.Generic;

namespace IOF.Models
{
    public class SearchResult<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public T[] Data { get; set; }
    }
}

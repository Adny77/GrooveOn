using System.Collections.Generic;

namespace GrooveOn.Model.ResponseObjects
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int? TotalCount { get; set; }
    }
}

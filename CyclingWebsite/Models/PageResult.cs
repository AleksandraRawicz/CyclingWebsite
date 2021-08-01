using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Models
{
    public class PageResult<T>
    {
        public List<T> Items { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public string Phrase { get; set; }

        public int PageNumber { get; set; }

        public PageResult(List<T> items, int total, int size, int page, string phrase)
        {
            Items = items;
            TotalItems = total;
            TotalPages = (int)Math.Ceiling(total /(double) size);
            PageSize = size;
            Phrase = phrase;
            PageNumber = page;
        }
    }
}

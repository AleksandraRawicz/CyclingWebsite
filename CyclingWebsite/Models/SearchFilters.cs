using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Models
{
    public class SearchFilters
    {
        public string SearchPhrase { get; set; }

        public int Size { get; set; } = 5;

        public int Page { get; set; } = 1;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FionPushFilm.Models
{
    public class SearchResult
    {
        public List<ResourceItem> Items
        {
            get;
            set;
        }

        public int PageCount
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImperialSanWPF.Models;

namespace ImperialSanWPF.Utils
{
    static class AvailiableSortItems
    {
        static public List<SortItem> sortItems = new List<SortItem>()
        {
            new SortItem()
            {
                DisplayText = "По умолчанию",
                SortText = null,
                SortOrder = null,
            },

            new SortItem()
            {
                DisplayText = "Сначала самые дорогие",
                SortText = "price",
                SortOrder = "desc",
            },
            new SortItem()
            {
                DisplayText = "Сначала самые дешёвые",
                SortText = "price",
                SortOrder = "asc",
            },

            new SortItem()
            {
                DisplayText = "По названию А до Я",
                SortText = "title",
                SortOrder = "desc",
            },
            new SortItem()
            {
                DisplayText = "По названию Я до А",
                SortText = "title",
                SortOrder = "asc",
            },

            new SortItem()
            {
                DisplayText = "Сначала самые новые",
                SortText = "date",
                SortOrder = "desc",
            },
            new SortItem()
            {
                DisplayText = "Сначала самые старые",
                SortText = "date",
                SortOrder = "asc",
            },
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace VSPoll.API.Models
{
    public class Page<TItem>
    {
        public int Size { get; }

        public int Number { get; }

        public int TotalItems { get; }

        public IEnumerable<TItem> Items { get; }

        public int TotalPages => Size > 0 ? (int)Math.Ceiling((decimal)TotalItems / Size) : 0;

        public Page(int size, int number, int totalItems, IEnumerable<TItem> items)
        {
            Size = size;
            Number = number;
            TotalItems = totalItems;
            Items = items;
        }

        public Page<TDestination> Map<TDestination>(Func<TItem, TDestination> mapFunction)
            => new Page<TDestination>(Size, Number, TotalItems, Items.Select(mapFunction));
    }
}

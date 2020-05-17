using System;
using System.Collections.Generic;
using System.Linq;

namespace VSPoll.API.Models
{
    public class Page<TItem>
    {
        public int Size { get; set; }

        public int Number { get; set; }

        public int TotalItems { get; set; }

        public IEnumerable<TItem> Items { get; set; } = null!;

        public int TotalPages => Size > 0 ? (int)Math.Ceiling((decimal)TotalItems / Size) : 0;

        public Page<TDestination> Map<TDestination>(Func<TItem, TDestination> mapFunction) => new Page<TDestination>
        {
            Items = Items.Select(mapFunction),
            Number = Number,
            Size = Size,
            TotalItems = TotalItems,
        };
    }
}

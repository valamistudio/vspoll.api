using System;
using System.Collections.Generic;
using System.Linq;

namespace VSPoll.API.Models.Output;

public record Page<TItem>(int Size, int Number, int TotalItems, IEnumerable<TItem> Items)
{
    public int TotalPages => Size > 0 ? (int)Math.Ceiling((decimal)TotalItems / Size) : 0;

    public Page<TDestination> Map<TDestination>(Func<TItem, TDestination> mapFunction)
        => new(Size, Number, TotalItems, Items.Select(mapFunction));
}

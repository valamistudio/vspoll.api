using System;
using System.Collections.Generic;
using System.Linq;

namespace VSPoll.API.Utils;

using OrderBy = Func<IEnumerable<NormalizationWrapper<IPercentage>>, Func<NormalizationWrapper<IPercentage>, decimal>, IOrderedEnumerable<NormalizationWrapper<IPercentage>>>;

public static class MathUtils
{
    private static decimal SortingPredicate<T>(NormalizationWrapper<T> wrapper)
        where T : IPercentage
        => wrapper.OriginalPercentage - Math.Truncate(wrapper.OriginalPercentage);

    public static IEnumerable<T> NormalizePercentages<T>(IEnumerable<T> percentages, int decimals = 0)
        where T : IPercentage
    {
        var wrappers = percentages.Select(percentage => new NormalizationWrapper<IPercentage>(percentage)).ToList();
        foreach (var percentage in percentages)
            percentage.Percentage = Math.Round(percentage.Percentage, decimals);

        var diff = 100 - percentages.Sum(percentage => percentage.Percentage);
        if (diff != 0)
        {
            var sign = Math.Sign(diff);
            var step = sign / (decimal)Math.Pow(10, decimals);
            var count = (int)(step / diff);
            var orderBy = sign == 1 ? (OrderBy)Enumerable.OrderByDescending : Enumerable.OrderBy;
            foreach (var wrapper in orderBy(wrappers, SortingPredicate).Take(count))
                wrapper.Percentage.Percentage += step;
        }
        return percentages;
    }
}

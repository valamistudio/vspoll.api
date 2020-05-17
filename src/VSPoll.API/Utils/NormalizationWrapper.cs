using VSPoll.API.Models;

namespace VSPoll.API.Utils
{
    public class NormalizationWrapper<T>
    where T : IPercentage
    {
        public T Percentage { get; }

        public decimal OriginalPercentage { get; }

        public NormalizationWrapper(T percentage)
        {
            this.Percentage = percentage;
            this.OriginalPercentage = percentage.Percentage;
        }
    }
}

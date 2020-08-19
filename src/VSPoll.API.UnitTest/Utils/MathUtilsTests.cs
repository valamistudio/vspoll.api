using System.Linq;
using FluentAssertions;
using VSPoll.API.Models.Output;
using VSPoll.API.Utils;
using Xunit;

namespace VSPoll.API.UnitTest.Utils
{
    public class MathUtilsTests
    {
        [Fact]
        public void NormalizePercentages_NoChanges()
        {
            var percentageA = 20;
            var optionA = new PollOption { Percentage = percentageA };

            var percentageB = 30;
            var optionB = new PollOption { Percentage = percentageB };

            var percentageC = 50;
            var optionC = new PollOption { Percentage = percentageC };

            var options = new[] { optionA, optionB, optionC }.ToList();
            MathUtils.NormalizePercentages(options);

            options.Count.Should().Be(3);

            options.Should().Contain(optionA);
            optionA.Percentage.Should().Be(percentageA);

            options.Should().Contain(optionB);
            optionB.Percentage.Should().Be(percentageB);

            options.Should().Contain(optionC);
            optionC.Percentage.Should().Be(percentageC);
        }

        [Fact]
        public void NormalizePercentages_Changes()
        {
            var percentages = new[]
            {
                18.562874251497007m,
                20.958083832335326m,
                18.562874251497007m,
                19.161676646706585m,
                22.75449101796407m,
            }.ToList();
            var expected = new[]
            {
                19,
                21,
                19,
                18,
                23,
            };

            var options = percentages.Select(percentage => new PollOption { Percentage = percentage }).ToList();
            MathUtils.NormalizePercentages(options);

            options.Count.Should().Be(percentages.Count);
            foreach (var item in expected)
                options.Select(option => option.Percentage).Should().Contain(item);
        }

        [Fact]
        public void NormalizePercentages_CustomStep()
        {
            var percentages = new[]
            {
                18.562874251497007m,
                20.958083832335326m,
                18.562874251497007m,
                19.161676646706585m,
                22.75449101796407m,
            }.ToList();
            var expected = new[]
            {
                18.56m,
                20.97m,
                18.56m,
                19.16m,
                22.75m,
            };

            var options = percentages.Select(percentage => new PollOption { Percentage = percentage }).ToList();
            MathUtils.NormalizePercentages(options, 2);

            options.Count.Should().Be(percentages.Count);
            foreach (var item in expected)
                options.Select(option => option.Percentage).Should().Contain(item);
        }
    }
}

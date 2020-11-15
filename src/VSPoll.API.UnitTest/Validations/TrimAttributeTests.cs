using System.ComponentModel.DataAnnotations;
using VSPoll.API.Validations;
using Xunit;

namespace VSPoll.API.UnitTest.Validations
{
    public class TrimAttributeTests
    {
        private class TestClass
        {
            [Trim]
            public string TestProperty { get; set; }
        }

        [Fact]
        public void TrimAttributeTest()
        {
            const string value = " foo ";
            const string trimmedValue = "foo";
            TrimAttribute attribute = new();
            TestClass test = new()
            {
                TestProperty = value,
            };
            ValidationContext context = new(test)
            {
                MemberName = nameof(TestClass.TestProperty),
            };
            attribute.GetValidationResult(value, context);
            Assert.Equal(test.TestProperty, trimmedValue);
        }
    }
}

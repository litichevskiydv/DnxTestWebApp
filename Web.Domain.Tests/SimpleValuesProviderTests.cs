namespace Web.Domain.Tests
{
    using NUnit.Framework;
    using ValuesProvider;

    [TestFixture]
    public class SimpleValuesProviderTests
    {
        private readonly SimpleValuesProvider _valuesProvider;

        public SimpleValuesProviderTests()
        {
            _valuesProvider = new SimpleValuesProvider();
        }

        [Test]
        public void ShouldReturnValueFromConfiguration()
        {
            // Given
            var configiration = new ValuesControllerConfiguration {Value = "aaa", Value1 = "bbb", Value2 = "ccc"};

            // When
            var v = _valuesProvider.GetValue(configiration);

            // Then
            Assert.AreEqual(configiration.Value, v);
        }
    }
}
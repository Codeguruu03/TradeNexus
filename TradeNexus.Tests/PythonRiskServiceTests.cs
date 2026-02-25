using Xunit;
using TradeNexus.Web.Services;

namespace TradeNexus.Tests
{
    public class PythonRiskServiceTests
    {
        [Fact]
        public void PythonRiskService_CanBeInstantiated()
        {
            // Arrange & Act
            var service = new PythonRiskService();

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void ExecuteRiskEngine_WithEmptyInput_ReturnsResult()
        {
            // Arrange
            var service = new PythonRiskService();
            var jsonInput = "{}";

            // Act
            var result = service.ExecuteRiskEngine(jsonInput);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}

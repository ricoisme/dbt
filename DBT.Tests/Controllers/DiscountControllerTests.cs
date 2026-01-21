using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Controllers;
using WebApplication1.Services;
using WebApplication1.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DBT.Tests.Controllers
{
    [TestClass]
    public class DiscountControllerTests
    {
        [TestMethod]
        public void ApplyDiscount_Returns_Ok_With_Correct_Values()
        {
            // Arrange
            var service = new MarketingDiscountService();
            var logger = NullLogger<DiscountController>.Instance;
            var controller = new DiscountController(service, logger);

            var request = new ApplyDiscountRequest
            {
                Amount = 100m,
                Level = "Gold"
            };

            // Act
            var actionResult = controller.ApplyDiscount(request);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");

            var response = okResult!.Value as ApplyDiscountResponse;
            Assert.IsNotNull(response, "Expected ApplyDiscountResponse in OkObjectResult.Value");

            var expectedMultiplier = service.GetDiscountMultiplier(MembershipLevel.Gold);
            var expectedDiscounted = service.ApplyDiscount(100m, MembershipLevel.Gold);

            Assert.AreEqual(100m, response!.OriginalAmount);
            Assert.AreEqual(expectedMultiplier, response.Multiplier);
            Assert.AreEqual(expectedDiscounted, response.DiscountedAmount);
        }
    }
}

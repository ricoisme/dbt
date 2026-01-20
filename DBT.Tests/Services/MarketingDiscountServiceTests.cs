using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Services;

namespace DBT.Tests.Services
{
    [TestClass]
    public class MarketingDiscountServiceTests
    {
        private readonly MarketingDiscountService _service = new MarketingDiscountService();

        [TestMethod]
        public void GetDiscountMultiplier_Returns_0_70_For_Platinum()
        {
            var multiplier = _service.GetDiscountMultiplier(MembershipLevel.Platinum);
            Assert.AreEqual(0.70m, multiplier);
        }

        [TestMethod]
        public void GetDiscountMultiplier_Returns_0_95_For_Basic()
        {
            var multiplier = _service.GetDiscountMultiplier(MembershipLevel.Basic);
            Assert.AreEqual(0.95m, multiplier);
        }

        [TestMethod]
        public void ApplyDiscount_Applies_Multiplier_And_Rounds_To_Two_Decimals()
        {
            decimal amount = 123.45m;
            // Silver multiplier = 0.8250 => 123.45 * 0.825 = 101.87125 => rounded to 101.87
            var discounted = _service.ApplyDiscount(amount, MembershipLevel.Silver);
            Assert.AreEqual(101.87m, discounted);
        }
    }
}

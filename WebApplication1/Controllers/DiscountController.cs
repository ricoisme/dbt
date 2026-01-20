using System;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IMarketingDiscountService _discountService;

        public DiscountController(IMarketingDiscountService discountService)
        {
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        }

        /// <summary>
        /// 取得指定會員等級的折扣乘數（例如 0.70 表示 7 折）
        /// 範例: GET /api/v1/discount/multiplier/Gold
        /// </summary>
        [HttpGet("multiplier/{level}")]
        public ActionResult<decimal> GetMultiplier(string level)
        {
            if (!Enum.TryParse<MembershipLevel>(level, true, out var membership))
                return BadRequest($"Invalid membership level: {level}");

            var multiplier = _discountService.GetDiscountMultiplier(membership);
            return Ok(multiplier);
        }

        public record ApplyDiscountRequest(decimal Amount, string Level);
        public record ApplyDiscountResponse(decimal OriginalAmount, decimal Multiplier, decimal DiscountedAmount);

        /// <summary>
        /// 套用折扣並回傳折後金額
        /// 範例: POST /api/v1/discount/apply  Body: { "amount":100, "level":"Platinum" }
        /// </summary>
        [HttpPost("apply")]
        public ActionResult<ApplyDiscountResponse> ApplyDiscount([FromBody] ApplyDiscountRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (request.Amount < 0)
                return BadRequest("Amount must be non-negative.");

            if (!Enum.TryParse<MembershipLevel>(request.Level, true, out var membership))
                return BadRequest($"Invalid membership level: {request.Level}");

            var multiplier = _discountService.GetDiscountMultiplier(membership);
            var discounted = _discountService.ApplyDiscount(request.Amount, membership);

            return Ok(new ApplyDiscountResponse(request.Amount, multiplier, discounted));
        }
    }
}

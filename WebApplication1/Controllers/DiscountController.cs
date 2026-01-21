using System;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.Models;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IMarketingDiscountService _discountService;
        private readonly ILogger<DiscountController> _logger;

        public DiscountController(IMarketingDiscountService discountService, ILogger<DiscountController> logger)
        {
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 取得指定會員等級的折扣乘數（例如 0.70 表示 7 折）
        /// 範例: GET /api/v1/discount/multiplier/Gold
        /// </summary>
        [HttpGet("multiplier/{level}")]
        public ActionResult<decimal> GetMultiplier(string level)
        {
            if (!Enum.TryParse<MembershipLevel>(level, true, out var membership))
            {
                _logger.LogWarning("GetMultiplier: invalid membership level: {Level}", level);
                return BadRequest($"Invalid membership level: {level}");
            }

            var multiplier = _discountService.GetDiscountMultiplier(membership);
            return Ok(multiplier);
        }

        // Request/response moved to Models/DiscountModels.cs
        /// <summary>
        /// 套用折扣並回傳折後金額
        /// 範例: POST /api/v1/discount/apply  Body: { "amount":100, "level":"Platinum" }
        /// </summary>
        [HttpPost("apply")]
        public ActionResult<ApplyDiscountResponse> ApplyDiscount([FromBody] ApplyDiscountRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("ApplyDiscount: request is null");
                return BadRequest("Request body is required.");
            }

            if (request.Amount < 0)
            {
                _logger.LogWarning("ApplyDiscount: negative amount: {Amount}", request.Amount);
                return BadRequest("Amount must be non-negative.");
            }

            if (string.IsNullOrWhiteSpace(request.Level) || !Enum.TryParse<MembershipLevel>(request.Level, true, out var membership))
            {
                _logger.LogWarning("ApplyDiscount: invalid membership level: {Level}", request.Level);
                return BadRequest($"Invalid membership level: {request.Level}");
            }

            var multiplier = _discountService.GetDiscountMultiplier(membership);
            var discounted = _discountService.ApplyDiscount(request.Amount, membership);

            return Ok(new ApplyDiscountResponse(request.Amount, multiplier, discounted));
        }
    }
}

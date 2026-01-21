using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ApplyDiscountRequest
    {
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be non-negative.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Level is required.")]
        public string? Level { get; set; }
    }

    public class ApplyDiscountResponse
    {
        public decimal OriginalAmount { get; set; }
        public decimal Multiplier { get; set; }
        public decimal DiscountedAmount { get; set; }

        public ApplyDiscountResponse(decimal originalAmount, decimal multiplier, decimal discountedAmount)
        {
            OriginalAmount = originalAmount;
            Multiplier = multiplier;
            DiscountedAmount = discountedAmount;
        }
    }
}

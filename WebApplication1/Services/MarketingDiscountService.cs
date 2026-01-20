using System;

namespace WebApplication1.Services
{
    /// <summary>
    /// 會員等級（由低到高）
    /// </summary>
    public enum MembershipLevel
    {
        Basic = 1,
        Bronze = 2,
        Silver = 3,
        Gold = 4,
        Platinum = 5
    }

    /// <summary>
    /// 提供行銷折扣計算的方法介面
    /// </summary>
    public interface IMarketingDiscountService
    {
        /// <summary>
        /// 取得指定會員等級的折扣乘數（例如 0.70 表示 7 折）
        /// </summary>
        decimal GetDiscountMultiplier(MembershipLevel level);

        /// <summary>
        /// 套用折扣於指定金額並回傳折後金額（四捨五入到小數點後兩位）
        /// </summary>
        decimal ApplyDiscount(decimal amount, MembershipLevel level);
    }

    /// <summary>
    /// 行銷折扣服務：支援 5 個等級，最高等級折扣 7 折（0.70），最低等級折扣 95 折（0.95），其餘等級採線性平衡分配。
    /// </summary>
    public class MarketingDiscountService : IMarketingDiscountService
    {
        private const int TotalLevels = 5;
        private static readonly decimal HighestMultiplier = 0.70m; // 7 折
        private static readonly decimal LowestMultiplier = 0.95m;  // 95 折（即 0.95）

        public decimal GetDiscountMultiplier(MembershipLevel level)
        {
            int index = (int)level - 1; // Basic -> 0, Platinum -> 4
            if (index < 0 || index >= TotalLevels)
                throw new ArgumentOutOfRangeException(nameof(level));

            decimal step = (LowestMultiplier - HighestMultiplier) / (TotalLevels - 1);
            // Levels increase from Basic (index 0) to Platinum (index TotalLevels-1).
            // We want Basic -> LowestMultiplier, Platinum -> HighestMultiplier.
            decimal multiplier = LowestMultiplier - (index * step);
            return Math.Round(multiplier, 4); // 保留小數四位以便後續計算
        }

        public decimal ApplyDiscount(decimal amount, MembershipLevel level)
        {
            if (amount < 0m)
                throw new ArgumentOutOfRangeException(nameof(amount), "金額不得為負值");

            decimal multiplier = GetDiscountMultiplier(level);
            decimal discounted = amount * multiplier;
            return Math.Round(discounted, 2, MidpointRounding.AwayFromZero);
        }
    }
}

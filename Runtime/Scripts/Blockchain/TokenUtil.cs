using System.Numerics;
using Nethereum.Util;

namespace ElympicsLobbyPackage.Blockchain
{
    public class TokenUtil
    {
        public static string Format(BigInteger amount, int decimals, string symbol)
        {
            if (symbol == null || decimals == 0)
                return "";
            return $"{Format(amount, decimals)} {symbol}";
        }

        public static string Format(BigInteger amount, int decimals)
        {
            if (decimals == 0)
                return "";
            var value = UnitConversion.Convert.FromWei(amount, decimals);
            return value.ToString("0.00");
        }
    }
}

using SCS;
using System.Numerics;
using System.Collections.Generic;
using ElympicsLobbyPackage.Blockchain;
using ElympicsLobbyPackage.Blockchain.Trust.Command;
using ElympicsLobbyPackage.Blockchain.Wallet;
using Nethereum.Util;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Blockchain.Communication;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Trust
{

    public class TrustIncreaseCommandFactory : ITrustIncreaseCommandFactory
    {

        private readonly TokenInfo tokenInfo;
        private readonly DepositInfo depositInfo;
        private readonly DuelInfo duelInfo;
        private readonly BalanceInfo balanceInfo;
        private readonly ChainConfig chainConfig;
        private readonly Web3Wallet wallet;

        public TrustIncreaseCommandFactory(ChainConfig chainConfig, Web3Wallet wallet, TokenInfo tokenInfo, DepositInfo depositInfo, DuelInfo duelInfo, BalanceInfo balanceInfo)
        {
            this.chainConfig = chainConfig;
            this.wallet = wallet;
            this.tokenInfo = tokenInfo;
            this.depositInfo = depositInfo;
            this.duelInfo = duelInfo;
            this.balanceInfo = balanceInfo;
        }

        public async UniTask<IEnumerable<TrustIncreaseCommand>> IncreaseTrustBy(BigInteger amount)
        {
            var duelAllowanceCondition = DuelAllowanceStrategy(amount);
            var trustAllowanceCondition = TrustAllowanceStrategy(chainConfig, wallet, amount);
            var depositCondition = new TrustIncreaseStrategyAlways();
            var builder = new TrustIncreaseBuilder();
            if (await duelAllowanceCondition.IsStepRequired())
            {
                builder.AddIncreaseDuelAllowance(chainConfig, wallet);
            }
            if (await trustAllowanceCondition.IsStepRequired())
            {
                builder.AddIncreaseTrustAllowance(chainConfig, wallet);
            }
            if (await depositCondition.IsStepRequired())
            {
                builder.AddDepositTrust(chainConfig, wallet, amount);
            }
            return builder.Build();
        }

        private ITrustIncreaseStrategy DuelAllowanceStrategy(BigInteger trustWantedDeposit)
        {
            var balanceBasedAllowance = BigInteger.Max(balanceInfo.Balance, 1); // Zero allowance is not accepted;
            var constantBasedAllowance = UnitConversion.Convert.ToWei(100m, tokenInfo.Decimals);
            var totalTrustAfterAction = depositInfo.Total + trustWantedDeposit;
            var expectedMinAllowance = BigInteger.Max(BigInteger.Min(balanceBasedAllowance, constantBasedAllowance), totalTrustAfterAction);
            return new TrustIncreaseStrategyRequiredAllowance(duelInfo.Allowance, expectedMinAllowance);
        }

        private ITrustIncreaseStrategy TrustAllowanceStrategy(ChainConfig chainConfig, Web3Wallet wallet, BigInteger trustWantedDeposit)
        {
            return new TrustIncreaseStrategyRequiredAllowance(async () => await wallet.GetAllowance(chainConfig.GetSmartContract(SmartContractType.SecurityDeposit).Address), trustWantedDeposit);
        }
    }
}

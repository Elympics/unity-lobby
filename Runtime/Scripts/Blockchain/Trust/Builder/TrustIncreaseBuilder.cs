using System;
using SCS;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using ElympicsLobbyPackage.Blockchain.Trust.Command;
using ElympicsLobbyPackage.Blockchain.Wallet;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    internal class TrustIncreaseBuilder : ITrustIncreaseBuilder
    {
        private TrustIncreaseMaxAllowanceDuelCommand? _duelAllowanceCommand = null;
        private TrustIncreaseMaxAllowanceTrustCommand? _trustAllowanceCommand = null;
        private TrustIncreaseDepositCommand? _depositTrustCommand = null;

        public void AddDepositTrust(ChainConfig chainConfig, Web3Wallet wallet, BigInteger amount)
        {
            if (_depositTrustCommand != null)
                throw new Exception("Deposit Trust command already added");
            _depositTrustCommand = new TrustIncreaseDepositCommand(chainConfig, wallet, amount);
        }

        public void AddIncreaseTrustAllowance(ChainConfig chainConfig, Web3Wallet wallet)
        {
            if (_trustAllowanceCommand != null)
                throw new Exception("Trust increase allowance command already added");
            _trustAllowanceCommand = new TrustIncreaseMaxAllowanceTrustCommand(chainConfig, wallet);
        }

        public void AddIncreaseDuelAllowance(ChainConfig chainConfig, Web3Wallet wallet)
        {
            if (_duelAllowanceCommand != null)
                throw new Exception("Duel increase allowance command already added");
            _duelAllowanceCommand = new TrustIncreaseMaxAllowanceDuelCommand(chainConfig, wallet);
        }

        public IEnumerable<TrustIncreaseCommand> Build()
        {
            return new List<TrustIncreaseCommand?> {
                _duelAllowanceCommand,
                _trustAllowanceCommand,
                _depositTrustCommand
            }.OfType<TrustIncreaseCommand>();
        }
    }
}
